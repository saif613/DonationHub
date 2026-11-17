using DonationHub.Application.Features.Auth.DTOs.Response;
using DonationHub.Application.Interfaces.Repositories;
using DonationHub.Application.Interfaces.Services;
using DonationHub.Domain.Common.Results;
using MediatR;

namespace DonationHub.Application.Features.Auth.Queries.GetCurrentUser;

public sealed class GetCurrentUserHandler: IRequestHandler<GetCurrentUserQuery, Result<CurrentUserResponseDto>>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;
    private readonly IUserRepository _userRepository;

    public GetCurrentUserHandler(ICurrentUserService currentUser, IIdentityService identityService, IUserRepository userRepository)
    {
        _currentUser = currentUser;
        _identityService = identityService;
        _userRepository = userRepository;
    }

    public async Task<Result<CurrentUserResponseDto>> Handle(GetCurrentUserQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;

        if (userId is null)
            return Errors.Auth.Unauthorized;

        var user = await _userRepository.GetByIdAsync(userId.Value, ct);

        if (user is null)
            return Errors.Auth.UserNotFound;

        var identityUser = await _identityService.FindByIdAsync(user.Id, ct);

        if (identityUser is null)
            return Errors.Auth.UserNotFound;

        var roles = await _identityService.GetRolesAsync(user.Id, ct);

        var response = new CurrentUserResponseDto(
            user.Id,
            identityUser.Email,
            user.FullName,
            user.PreferredLanguage,
            roles.ToList()
        );

        return Result.Success(response);
    }
}