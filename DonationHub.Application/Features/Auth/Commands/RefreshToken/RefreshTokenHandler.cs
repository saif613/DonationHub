using DonationHub.Application.Features.Auth.DTOs.Response;
using DonationHub.Application.Interfaces.Repositories;
using DonationHub.Application.Interfaces.Services;
using DonationHub.Application.Interfaces.UnitOfWork;
using DonationHub.Application.Options;
using DonationHub.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Options;
using RefreshTokenEntity = DonationHub.Domain.Entities.RefreshToken;

namespace DonationHub.Application.Features.Auth.Commands.RefreshToken;

public sealed class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, Result<TokenResponseDto>>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IOptions<JwtOptions> _jwtOptions;
    private readonly IIdentityService _identityService;
  

    public RefreshTokenHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, TimeProvider timeProvider, IJwtTokenService jwtTokenService, IRefreshTokenRepository refreshTokenRepository, IOptions<JwtOptions> jwtOptions, IIdentityService identityService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
        _jwtTokenService = jwtTokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtOptions = jwtOptions;
        _identityService = identityService;
    }

    private DateTime GetUtcNow() => _timeProvider.GetUtcNow().UtcDateTime;
    public async Task<Result<TokenResponseDto>> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var tokenHash = Helpers.RefreshTokenHelper.Hash(request.RefreshToken);

        var oldRefreshToken = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash, ct);

        var utcNow = GetUtcNow();

        if (oldRefreshToken is null || !oldRefreshToken.IsActive(utcNow))
            return Errors.Auth.RefreshTokenInvalid;

        var userId = oldRefreshToken.UserId;

        var user = await _userRepository.GetByIdAsync(userId, ct);

        if (user is null)
            return Errors.Auth.UserNotFound;

        if (!user.IsActive)
            return Errors.Auth.AccountDeactivated;

        var applicationUser = await _identityService.FindByIdAsync(userId, ct);

        if (applicationUser is null)
            return Errors.Auth.UserNotFound;

        var roles = await _identityService.GetRolesAsync(userId, ct);

        var accessTokenExpiresAt = utcNow.AddMinutes(_jwtOptions.Value.ExpiryMinutes);

        var accessToken = _jwtTokenService.GenerateJwtToken(userId, applicationUser.Email, applicationUser.UserName, user.FullName, roles);

        var newRefreshToken = Helpers.RefreshTokenHelper.Generate();

        var newRefreshTokenHash = Helpers.RefreshTokenHelper.Hash(newRefreshToken);

        var newRefreshTokenEntity = RefreshTokenEntity.Create(
            applicationUser.Id,
            newRefreshTokenHash,
            utcNow,
            utcNow.AddDays(_jwtOptions.Value.RefreshTokenExpiryDays),
            userId
        );
        return await _unitOfWork.ExcuteInTRansactionAsync(async () =>
        {
            oldRefreshToken.ReplaceWith(newRefreshTokenEntity.Id, utcNow, userId);

            await _refreshTokenRepository.AddAsync(newRefreshTokenEntity, ct);

            await _unitOfWork.SaveChangesAsync(ct);

            var response = new TokenResponseDto(accessToken, newRefreshToken, accessTokenExpiresAt, newRefreshTokenEntity.ExpiresAt);

            return Result.Success(response);
        }, ct);            
    }
}
