using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Common.Results
{
    public class Result
    {
        public bool IsSuccess { get; }
        public Error Error { get; }
        public bool IsFailure => !IsSuccess;

       public IReadOnlyDictionary<string, string[]> ValidationErrors { get; }

        protected Result(bool isSuccess, Error error, IReadOnlyDictionary<string, string[]>? validationErrors = null)
        {
            ArgumentNullException.ThrowIfNull(error);

            if (isSuccess && error != Error.None)
                throw new InvalidOperationException("A successful result cannot contain an error");

            if (!isSuccess && error == Error.None)
                throw new InvalidOperationException("A failed result must contain an errorr");
            IsSuccess = isSuccess;
            Error = error;

            ValidationErrors = validationErrors ?? new Dictionary<string, string[]>();
        }

        public static Result Success() => new(true, Error.None); 
        public static Result Failure(Error error, IReadOnlyDictionary<string, string[]>? validationErrors = null) => new(false, error, validationErrors);

        public static Result<T> Success<T>(T value) => new(value, true, Error.None); 
        public static Result<T> Failure<T>(Error error, IReadOnlyDictionary<string, string[]>? validationErrors = null) => new(default, false, error, validationErrors);

        public static implicit operator Result(Error error) => Failure(error);

    }

    public sealed class Result<T>: Result
    {
        private readonly T? _value;
        internal Result(T? value, bool isSuccess, Error error, IReadOnlyDictionary<string, string[]>? validationErrors = null) : base(isSuccess, error, validationErrors) => _value = value;
        public T Value => IsSuccess ? _value! : throw new InvalidOperationException("Cannot access Value of a failed result");

        public static implicit operator Result<T>(T value) => Success(value);
        public static implicit operator Result<T>(Error error) => Failure<T>(error);
    }
}
