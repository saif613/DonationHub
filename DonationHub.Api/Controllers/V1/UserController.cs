using Asp.Versioning;
using DonationHub.Api.Extensions;
using DonationHub.Application.Features.Users.Commands.ActivateUser;
using DonationHub.Application.Features.Users.Commands.ChangeLanguage;
using DonationHub.Application.Features.Users.Commands.DeactivateUser;
using DonationHub.Application.Features.Users.Commands.UpdateProfile;
using DonationHub.Application.Features.Users.DTOs;
using DonationHub.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DonationHub.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost("{userId:guid}/activate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails) ,StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ActivateUser(Guid userId, CancellationToken ct = default)
        {
            var result = await _mediator.Send(new ActivateUserCommand(userId), ct);

            if (result.IsFailure)
                return result.ToProblem(HttpContext);

            return NoContent();
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost("{userId:guid}/deactivate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeactivateUser(Guid userId, CancellationToken ct = default)
        {
            var result = await _mediator.Send(new DeactivateUserCommand(userId), ct);

            if (result.IsFailure)
                return result.ToProblem(HttpContext);

            return NoContent();
        }

        [Authorize]
        [HttpPatch("me/language")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserResponseDto>> ChangeLanguage([FromBody] string language, CancellationToken ct = default)
        {
            var result = await _mediator.Send(new ChangeLanguageCommand(language), ct);

            if (result.IsFailure)
                return result.ToProblem(HttpContext);

            return Ok(result.Value);
        }

        [Authorize]
        [HttpPut("profile")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<UserResponseDto>> UpdateProfile([FromBody] UpdateProfileCommand command, CancellationToken ct = default)
        {
            var result = await _mediator.Send(command, ct);

            if (result.IsFailure)
                return result.ToProblem(HttpContext);

            return Ok(result.Value);
        }
    }
}
