using Asp.Versioning;
using Azure.Core;
using DonationHub.Application.DTOs.Requests;
using DonationHub.Application.DTOs.Responses;
using DonationHub.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DonationHub.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status201Created)]
        public async Task<ActionResult> Register(RegisterRequestDto request, CancellationToken ct = default)
        {
            await _authService.RegisterAsync(request, ct);
            return StatusCode(StatusCodes.Status201Created);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<TokenResponseDto>> Login(LoginRequestDto request, CancellationToken ct = default)
        {
            var response = await _authService.LoginAsync(request, ct);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<TokenResponseDto>> Refresh(RefreshTokenRequestDto request, CancellationToken ct = default)
        {
            var response = await _authService.RefreshAsync(request, ct);
            return Ok(response);
        }

        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Logout(LogoutRequestDto request, CancellationToken ct = default)
        {
            await _authService.LogoutAsync(request, ct);
            return NoContent();
        }

        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(typeof(CurrentUserResponseDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<CurrentUserResponseDto>> GetCurrentUser(CancellationToken ct = default)
        {
            var response = await _authService.GetCurrentUserAsync(ct);
            return Ok(response);
        }

        [Authorize]
        [HttpPut("Change-password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> ChangePassword(ChangePasswordRequestDto request, CancellationToken ct = default)
        {
            await _authService.ChangePasswordAsync(request, ct);

            return NoContent();
        }

    }
}
