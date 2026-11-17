using Asp.Versioning;
using DonationHub.Api.Extensions;
using DonationHub.Application.Features.Users.Commands.CreateStaff;
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
    [Authorize(Roles = Roles.Admin)]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("staff")]
        [ProducesResponseType(typeof(CreateStaffResponseDto), StatusCodes.Status201Created)]
        public async Task<ActionResult<CreateStaffResponseDto>> CreateStaff([FromBody] CreateStaffCommand command, CancellationToken ct = default)
        {
            var result = await _mediator.Send(command, ct);

            if (result.IsFailure)
                return result.ToProblem(HttpContext);

            return StatusCode(StatusCodes.Status201Created, result.Value);
        }
    }
}
