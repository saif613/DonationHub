using DonationHub.Application.Features.Users.DTOs;
using DonationHub.Application.Features.Users.UserMapper;
using DonationHub.Application.Interfaces.Repositories;
using DonationHub.Application.Interfaces.Services;
using DonationHub.Application.Interfaces.UnitOfWork;
using DonationHub.Domain.Common.Results;
using MediatR;

namespace DonationHub.Application.Features.Users.Commands.UpdateProfile;

public sealed class UpdateProfileHandler: IRequestHandler<UpdateProfileCommand, Result<UserResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;

    public UpdateProfileHandler(IUserRepository userRepository, IIdentityService identityService, ICurrentUserService currentUser, IUnitOfWork unitOfWork, TimeProvider timeProvider)
    {
        _userRepository = userRepository; 
        _identityService = identityService; 
        _currentUser = currentUser; 
        _unitOfWork = unitOfWork; 
        _timeProvider = timeProvider; 
    }

    private DateTime GetUtcNow() => _timeProvider.GetUtcNow().UtcDateTime;

    public async Task<Result<UserResponseDto>> Handle(UpdateProfileCommand request, CancellationToken ct)
    {
        var newEmail = request.Email!.Trim();

        var userId = _currentUser.UserId;

        if (userId is null)
            return Errors.Auth.Unauthorized;

        var user = await _userRepository.GetByIdAsync(userId.Value, ct);

        if (user is null)
            return Errors.User.NotFound;


        return await _unitOfWork.ExcuteInTRansactionAsync(async () =>
        {
            var emailResult = await _identityService.UpdateEmailAsync(user.Id, newEmail, ct);

            if (emailResult.IsFailure)
                return emailResult.Error!;

            user.UpdateProfile(request.FullName, GetUtcNow(), userId);

            await _unitOfWork.SaveChangesAsync(ct);

            var updatedIdentityUser = await _identityService.FindByIdAsync(user.Id, ct);

            if (updatedIdentityUser is null)
                return Errors.Auth.UserNotFound;

            var response = Mapper.ToResponse(user, updatedIdentityUser);

            return Result.Success(response);
        }, ct);
    }
}
