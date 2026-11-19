using Asp.Versioning;
using DonationHub.Api.Extensions;
using DonationHub.Application.Features.Campaigns.Commands.CreateCampaign;
using DonationHub.Application.Features.Campaigns.Commands.DeleteCampaign;
using DonationHub.Application.Features.Campaigns.Commands.UpdateCampaign;
using DonationHub.Application.Features.Campaigns.DTOs.Response;
using DonationHub.Application.Features.Campaigns.Queries.GetAllCampaign;
using DonationHub.Application.Features.Campaigns.Queries.GetAllCampaignSuccessStories;
using DonationHub.Application.Features.Campaigns.Queries.GetCampaignById;
using DonationHub.Application.Features.Campaigns.Response.DTOs;
using DonationHub.Application.Features.Categories.Commands.DeleteCategory;
using DonationHub.Application.Features.Categories.Commands.UpdateCategory;
using DonationHub.Application.Features.Categories.Queries.GetAllCategories;
using DonationHub.Application.Features.Category.DTOs.Responses;
using DonationHub.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace DonationHub.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Admin)]
    public class CampaignsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CampaignsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(CampaignResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CampaignResponseDto>> Create(CreateCampaignCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);

            if (result.IsFailure)
                return result.ToProblem(HttpContext);

            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }

        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(CampaignDetailsResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CampaignDetailsResponseDto>> GetById(Guid id, CancellationToken ct)
        {
            var language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

            var result = await _mediator.Send(new GetCampaignByIdQuery(id, language), ct);

            if (result.IsFailure)
                return result.ToProblem(HttpContext);

            return Ok(result.Value);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(UpdateResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<UpdateResponseDto>> Update([FromRoute] Guid id, [FromBody] UpdateCampaignCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command with { Id = id }, ct);

            if (result.IsFailure)
                return result.ToProblem(HttpContext);

            return Ok(result.Value);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
        {
            var result = await _mediator.Send(new DeleteCampaignCommand(id), ct);

            if (result.IsFailure)
                return result.ToProblem(HttpContext);

            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<CampaignListItemResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<CampaignListItemResponseDto>>> GetAll(CancellationToken ct)
        {
            var language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

            var result = await _mediator.Send(new GetAllCampaignsQuery(language), ct);

            if (result.IsFailure)
                return result.ToProblem(HttpContext);

            return Ok(result.Value);
        }

        [AllowAnonymous]
        [HttpGet("success-stories")]
        [ProducesResponseType(typeof(IReadOnlyList<CampaignListItemResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<CampaignListItemResponseDto>>> GetAllSuccessStories(CancellationToken ct)
        {
            var language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

            var result = await _mediator.Send(new GetAllCampaignSuccessStoriesQuery(language), ct);

            if (result.IsFailure)
                return result.ToProblem(HttpContext);

            return Ok(result.Value);
        }

    }
}
