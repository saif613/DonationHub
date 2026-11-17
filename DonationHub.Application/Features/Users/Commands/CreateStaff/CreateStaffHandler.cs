using DonationHub.Application.Features.Users.DTOs;
using DonationHub.Application.Interfaces.Repositories;
using DonationHub.Application.Interfaces.Services;
using DonationHub.Application.Interfaces.UnitOfWork;
using DonationHub.Domain.Common.Results;
using DonationHub.Domain.Constants;
using DonationHub.Domain.Entities;
using MediatR;

namespace DonationHub.Application.Features.Users.Commands.CreateStaff;

public sealed class CreateStaffHandler : IRequestHandler<CreateStaffCommand, Result<CreateStaffResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identity;
    private readonly TimeProvider _timeProvider;
    private readonly ICurrentUserService _currentUser;
    private readonly IUserRepository _userRepository;

    public CreateStaffHandler(IUnitOfWork unitOfWork, IIdentityService identity, TimeProvider timeProvider, ICurrentUserService currentUser, IUserRepository userRepository)
    {
        _unitOfWork = unitOfWork;
        _identity = identity;
        _timeProvider = timeProvider;
        _currentUser = currentUser;
        _userRepository = userRepository;
    }
    private DateTime GetUtcNow() => _timeProvider.GetUtcNow().UtcDateTime;
    public async Task<Result<CreateStaffResponseDto>> Handle(CreateStaffCommand request, CancellationToken ct)
    {
        var role = request.Role.Trim();

        var isAdmin = role.Equals(Roles.Admin, StringComparison.OrdinalIgnoreCase);

        var isCampaignManager = role.Equals(Roles.CampaignManager, StringComparison.OrdinalIgnoreCase);

        if (!isAdmin && !isCampaignManager)
            return Errors.Auth.RoleNotFound;

        role = isAdmin ? Roles.Admin : Roles.CampaignManager;

        var existingUser = await _identity.FindByEmailAsync(request.Email, ct);

        if (existingUser is not null)
            return Errors.Auth.EmailAlreadyExists;

        var userId = Guid.NewGuid();

        var temporaryPassword = Helpers.RefreshTokenHelper.Generate();        

         return await _unitOfWork.ExcuteInTRansactionAsync(async () =>
        {      
            var createResult = await _identity.CreateUserAsync(userId, request.Email, temporaryPassword, ct);

            if (createResult.IsFailure)
                return createResult.Error!;


            var roleResult = await _identity.AddToRoleAsync(userId, role, ct);

            if (roleResult.IsFailure)
                return roleResult.Error!;

            var now = GetUtcNow();

            var user = User.Create(userId, request.FullName, request.PreferredLanguage, now, _currentUser.UserId);

            await _userRepository.AddAsync(user, ct);

            await _unitOfWork.SaveChangesAsync(ct);

            var response = new CreateStaffResponseDto(user.Id, request.Email, user.FullName, role, temporaryPassword);

            return Result.Success(response);
        }, ct);
    }
}
