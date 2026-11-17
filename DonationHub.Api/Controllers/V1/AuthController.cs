using Asp.Versioning;
using DonationHub.Api.Extensions;
using DonationHub.Application.Features.Auth.Commands.ChangePassword;
using DonationHub.Application.Features.Auth.Commands.Login;
using DonationHub.Application.Features.Auth.Commands.Logout;
using DonationHub.Application.Features.Auth.Commands.RefreshToken;
using DonationHub.Application.Features.Auth.Commands.Register;
using DonationHub.Application.Features.Auth.DTOs.Response;
using DonationHub.Application.Features.Auth.Queries.GetCurrentUser;
using DonationHub.Application.Features.DTOs.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TokenResponseDto = DonationHub.Application.Features.Auth.DTOs.Response.TokenResponseDto;

namespace DonationHub.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult> Register([FromBody] RegisterCommand command, CancellationToken ct = default)
        {
            var result = await _mediator.Send(command, ct);

            if (result.IsFailure)
                return result.ToProblem(HttpContext);

            return NoContent();
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginCommand command, CancellationToken ct = default)
        {
            var result = await _mediator.Send(command, ct);

            if (result.IsFailure)
                return result.ToProblem(HttpContext);

            return Ok(result.Value);
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<TokenResponseDto>> Refresh([FromBody] RefreshTokenCommand command, CancellationToken ct = default)
        {
            var result = await _mediator.Send(command, ct);

            if (result.IsFailure)
                return result.ToProblem(HttpContext);

            return Ok(result.Value);
        }

        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> Logout([FromBody] LogoutCommand command, CancellationToken ct = default)
        {
            var result = await _mediator.Send(command, ct);

            if (result.IsFailure)
                return result.ToProblem(HttpContext);

            return NoContent();
        }

        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(typeof(CurrentUserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<CurrentUserResponseDto>> GetCurrentUser(CancellationToken ct = default)
        {
            var result = await _mediator.Send(new GetCurrentUserQuery(), ct);

            if (result.IsFailure)
                return result.ToProblem(HttpContext);

            return Ok(result.Value);
        }

        [Authorize]
        [HttpPut("Change-password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordCommand command, CancellationToken ct = default)
        {
            var result = await _mediator.Send(command, ct);

            if (result.IsFailure)
                return result.ToProblem(HttpContext);

            return NoContent();
        }

    }
}
