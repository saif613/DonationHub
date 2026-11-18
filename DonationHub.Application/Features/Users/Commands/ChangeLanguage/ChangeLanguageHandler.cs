using DonationHub.Application.Features.Users.DTOs;
using DonationHub.Application.Features.Users.UserMapper;
using DonationHub.Application.Interfaces.Repositories;
using DonationHub.Application.Interfaces.Services;
using DonationHub.Application.Interfaces.UnitOfWork;
using DonationHub.Domain.Common.Results;
using MediatR;

namespace DonationHub.Application.Features.Users.Commands.ChangeLanguage;

public sealed class ChangeLanguageHandler:IRequestHandler<ChangeLanguageCommand, Result<UserResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly TimeProvider _timeProvider;
    private readonly IIdentityService _identityService;
    public ChangeLanguageHandler(IUnitOfWork unitOfWork, IUserRepository userRepository, ICurrentUserService currentUser, TimeProvider timeProvider, IIdentityService identityService)
    {
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _currentUser = currentUser;
        _timeProvider = timeProvider;
        _identityService = identityService;
    }

    private DateTime GetUtcNow() => _timeProvider.GetUtcNow().UtcDateTime;

    public async Task<Result<UserResponseDto>> Handle(ChangeLanguageCommand request, CancellationToken ct)
    {
        var language = request.Language.Trim().ToLower();

        var userId = _currentUser.UserId;

        if (userId is null)
            return Errors.Auth.Unauthorized;

        var user = await _userRepository.GetByIdAsync(userId.Value, ct);

        if (user is null)
            return Errors.User.NotFound;

        var identityUser = await _identityService.FindByIdAsync(user.Id, ct);

        if (identityUser is null)
            return Errors.Auth.UserNotFound;

        user.ChangePreferredLanguage(language, GetUtcNow(), userId);

        await _unitOfWork.SaveChangesAsync(ct);

        var result = Mapper.ToResponse(user, identityUser);

        return Result.Success(result);
    }
}
