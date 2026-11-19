using Asp.Versioning;
using DonationHub.Api.Extensions;
using DonationHub.Application.Features.Categories.Commands.CreateCategory;
using DonationHub.Application.Features.Categories.Commands.DeleteCategory;
using DonationHub.Application.Features.Categories.Commands.UpdateCategory;
using DonationHub.Application.Features.Categories.Queries.GetAllCategories;
using DonationHub.Application.Features.Categories.Queries.GetCategoryById;
using DonationHub.Application.Features.Categories.Queries.GetPagedCategories;
using DonationHub.Application.Features.Category.DTOs.Responses;
using DonationHub.Application.Features.DTOs.Responses;
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
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<CategoryResponseDto>), StatusCodes.Status200OK)]        
        public async Task<ActionResult<IReadOnlyList<CategoryResponseDto>>> GetAll(CancellationToken ct = default)
        {
            var result = await _mediator.Send(new GetAllCategoriesQuery(), ct);

            if (result.IsFailure)
                return result.ToProblem(HttpContext);

            return Ok(result.Value);
        }

        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryResponseDto>> GetById(Guid id, CancellationToken ct = default)
        {
            var result = await _mediator.Send(new GetCategoryByIdQuery(id), ct);

            if (result.IsFailure)
                return result.ToProblem(HttpContext);

            return Ok(result.Value);
        }

        [HttpPost]
        [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails) , StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CategoryResponseDto>> Create([FromBody] CreateCategoryCommand command, CancellationToken ct = default)
        {
            var result = await _mediator.Send(command, ct);

            if (result.IsFailure)
                return result.ToProblem(HttpContext);

            return CreatedAtAction(nameof(GetById), new { id = result.Value.id, version = "1.0" }, result.Value);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)] 
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CategoryResponseDto>> Update(Guid id, [FromBody] UpdateCategoryCommand command, CancellationToken ct = default)
        {
            var result = await _mediator.Send(command with {Id = id}, ct);

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
        public async Task<ActionResult> Delete(Guid id, CancellationToken ct = default)
        {
           var result = await _mediator.Send(new DeleteCategoryCommand(id), ct);

            if (result.IsFailure)
                return result.ToProblem(HttpContext);

            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet("paged")]
        [ProducesResponseType(typeof(PagedResponse<CategoryResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails) , StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<CategoryResponseDto>>> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10,CancellationToken ct = default)
        {
            var result = await _mediator.Send(new GetPagedCategoriesQuery(pageNumber, pageSize), ct);

            if (result.IsFailure)
                return result.ToProblem(HttpContext);

            return Ok(result.Value);
        }
    }
}
