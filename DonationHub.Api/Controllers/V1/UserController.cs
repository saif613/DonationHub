using Asp.Versioning;
using DonationHub.Application.DTOs.Requests;
using DonationHub.Application.DTOs.Responses;
using DonationHub.Application.Interfaces.Services;
using DonationHub.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DonationHub.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost("{userId:guid}/activate")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserResponseDto>> ActivateUser(Guid userId, CancellationToken ct = default)
        {
            await _userService.ActivateUserAsync(userId, ct);
            return NoContent();
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost("{userId:guid}/deactivate")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<UserResponseDto>> DeactivateUser(Guid userId, CancellationToken ct = default)
        {
            await _userService.DeactivateUserAsync(userId, ct);
            return NoContent();
        }

        [Authorize]
        [HttpPatch("me/language")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]

        public async Task<ActionResult<UserResponseDto>> ChangeLanguage(Guid userId,[FromQuery] string language, CancellationToken ct = default)
        {
            var updatedUser = await _userService.ChangeUserLanguageAsync(userId, language, ct);
            return Ok(updatedUser);
        }

        [Authorize]
        [HttpPut("profile")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<UserResponseDto>> UpdateProfile([FromBody] UpdateProfileRequestDto request, CancellationToken ct = default)
        {
            var result = await _userService.UpdateProfileAsync(request, ct);

            return Ok(result);
        }
    }
}
