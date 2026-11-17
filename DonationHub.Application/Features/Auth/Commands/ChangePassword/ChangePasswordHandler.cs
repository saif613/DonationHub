using DonationHub.Application.Interfaces.Repositories;
using DonationHub.Application.Interfaces.Services;
using DonationHub.Application.Interfaces.UnitOfWork;
using DonationHub.Domain.Common.Results;
using MediatR;

namespace DonationHub.Application.Features.Auth.Commands.ChangePassword;

public sealed class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;
    

    public ChangePasswordHandler(IUnitOfWork unitOfWork, TimeProvider timeProvider, ICurrentUserService currentUser, IRefreshTokenRepository refreshTokenRepository, IIdentityService identityService)
    {
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
        _currentUser = currentUser;
        _refreshTokenRepository = refreshTokenRepository;
        _identityService = identityService;
    }

    private DateTime GetUtcNow() => _timeProvider.GetUtcNow().UtcDateTime;
    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;

        if (userId is null)
            return Errors.Auth.Unauthorized;

        return await _unitOfWork.ExcuteInTRansactionAsync(async () =>
        {
            var result = await _identityService.ChangePasswordAsync(userId.Value, request.CurrentPassword, request.NewPassword, ct);

            if (result.IsFailure)
                return result;

            var now = GetUtcNow();

            var activeTokens = await _refreshTokenRepository.GetActiveByUserIdAsync(userId.Value, now, ct);

            foreach (var token in activeTokens)
                token.Revoke(now, userId);

            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success();
        }, ct);
    }
}
