using DonationHub.Application.Interfaces.Repositories;
using DonationHub.Application.Interfaces.Services;
using DonationHub.Application.Interfaces.UnitOfWork;
using DonationHub.Domain.Common.Results;
using MediatR;

namespace DonationHub.Application.Features.Auth.Commands.Logout;

public sealed class LogoutHandler : IRequestHandler<LogoutCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly TimeProvider _timeProvider;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public LogoutHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser, TimeProvider timeProvider, IRefreshTokenRepository refreshTokenRepository)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _timeProvider = timeProvider;
        _refreshTokenRepository = refreshTokenRepository;
    }

    private DateTime GetUtcNow() => _timeProvider.GetUtcNow().UtcDateTime;
    public async Task<Result> Handle(LogoutCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;

        if (userId is null)
            return Errors.Auth.Unauthorized;

        var tokenHash = Helpers.RefreshTokenHelper.Hash(request.RefreshToken);

        var refreshToken = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash, ct);

        var utcNow = GetUtcNow();

        if (refreshToken is null || !refreshToken.IsActive(utcNow) || refreshToken.UserId != userId.Value)
            return Errors.Auth.RefreshTokenInvalid;

        refreshToken.Revoke(utcNow, userId.Value);

        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
