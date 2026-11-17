using DonationHub.Application.Interfaces.Repositories;
using DonationHub.Application.Interfaces.Services;
using DonationHub.Application.Interfaces.UnitOfWork;
using DonationHub.Domain.Common.Results;
using DonationHub.Domain.Constants;
using DonationHub.Domain.Entities;
using MediatR;

namespace DonationHub.Application.Features.Auth.Commands.Register;

public sealed class RegisterHandler : IRequestHandler<RegisterCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;
    private readonly IIdentityService _identityService;
    private readonly IUserRepository _userRepository;

    public RegisterHandler(IUnitOfWork unitOfWork, TimeProvider timeProvider, IIdentityService identityService, IUserRepository userRepository)
    {
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
        _identityService = identityService;
        _userRepository = userRepository;
    }

    private DateTime GetUtcNow() => _timeProvider.GetUtcNow().UtcDateTime;
    public async Task<Result> Handle(RegisterCommand request, CancellationToken ct = default)
    {
        var now = GetUtcNow();
        var newId = Guid.NewGuid();

        if (await _identityService.FindByEmailAsync(request.Email) is not null)
            return Errors.Auth.EmailAlreadyExists;
        return await _unitOfWork.ExcuteInTRansactionAsync(async () =>
        {           
            var createResult = await _identityService.CreateUserAsync(newId, request.Email, request.Password);

            if (createResult.IsFailure)
                return createResult;

            var roleResult = await _identityService.AddToRoleAsync(newId, Roles.Donor);

            if (roleResult.IsFailure)
                return roleResult;
          
            var user = User.Create(newId, request.FullName, request.PreferredLanguage, now, null);

            await _userRepository.AddAsync(user, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            return Result.Success();

        }, ct);          

    }
}
