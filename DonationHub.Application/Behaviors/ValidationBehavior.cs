using DonationHub.Domain.Common.Results;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace DonationHub.Application.Behaviors
{
    public class ValidationBehavior<TRequest, Tresponse> : IPipelineBehavior<TRequest, Tresponse> where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validator;
        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validator)
        {
            _validator = validator;
        }

        public async Task<Tresponse> Handle(TRequest request, RequestHandlerDelegate<Tresponse> next, CancellationToken ct)
        {
            if (!_validator.Any())
                return await next();

            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(_validator.Select(validator => validator.ValidateAsync(context, ct)));

            var failuers = validationResults
                .SelectMany(result => result.Errors)
                .Where(failuers => failuers is not null)
                .ToList();

            if (failuers.Count == 0)
                return await next();

            var propertyErrors = failuers
                .GroupBy(failuers => failuers.PropertyName)
                .ToDictionary(group => group.Key, group => group
                .Select(failuers => failuers.ErrorMessage)
                .ToArray());

            return CreateValidationResult(propertyErrors);
        }

        private static Tresponse CreateValidationResult(IReadOnlyDictionary<string, string[]> propertyErrors)
        {
            if(typeof(Tresponse) == typeof(Result))
                return (Tresponse)(object)Result.Failure(Errors.Validation.Failed, propertyErrors);
            
            var responseType = typeof(Tresponse);

            if(responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
            {
                var valueType = responseType.GetGenericArguments()[0];

                var failureMethod = typeof(Result)
                    .GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Single(method => method.Name == nameof(Result.Failure) && method.IsGenericMethodDefinition);

                var result = failureMethod
                    .MakeGenericMethod(valueType)
                    .Invoke(null, new object[]
                    {
                        Errors.Validation.Failed,
                        propertyErrors
                    });

                return (Tresponse)result!;
            }

            throw new InvalidOperationException($"Unsupported response type: {responseType.Name}");
        }
    }
}
