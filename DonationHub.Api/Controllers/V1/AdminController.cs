using Asp.Versioning;
using DonationHub.Application.Features.DTOs.Requests;
using DonationHub.Application.Features.DTOs.Responses;
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
    [Authorize(Roles = Roles.Admin)]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        public AdminController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("staff")]
        [ProducesResponseType(typeof(CreateStaffResponseDto), StatusCodes.Status201Created)]
        public async Task<ActionResult<CreateStaffResponseDto>> CreateStaff([FromBody] CreateStaffRequestDto request, CancellationToken ct = default)
        {
            var response = await _userService.CreateStaffAsync(request, ct);
            return StatusCode(StatusCodes.Status201Created, response);
        }
    }
}
