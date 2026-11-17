using DonationHub.Application.Exceptions;
using DonationHub.Application.Features.DTOs.Responses;
using DonationHub.Application.Interfaces.Repositories;
using DonationHub.Application.Interfaces.Services;
using DonationHub.Application.Interfaces.UnitOfWork;
using DonationHub.Domain.Common.Results;
using MediatR;

namespace DonationHub.Application.Features.Auth.Queries.GetCurrentUser;

public sealed class GetCurrentUserHandler : IRequestHandler<GetCurrentUserQuery, Result<CurrentUserResponseDto>>
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
    public async Task<Result<CurrentUserResponseDto>> Handle(GetCurrentUserQuery request, CancellationToken ct = default)
    {
        var userId = _currentUser.UserId;

        if (userId is null)
            return Errors.Auth.Unauthorized;


        var user = await _userRepository.GetByIdAsync(userId.Value, ct);

        if (user == null)
            return Errors.Auth.UserNotFound;

        var applicationUser = await _identityService.FindByIdAsync(user.Id);

        if (applicationUser is null)
            return Errors.Auth.UserNotFound;


        var roles = await _identityService.GetRolesAsync(user.Id);

        return new CurrentUserResponseDto(
            user.Id,
            applicationUser.Email,
            user.FullName,
            user.PreferredLanguage,
            roles.ToList()
        );
    }
}
