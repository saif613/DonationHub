using Asp.Versioning;
using DonationHub.Application.Features.Categories.Commands.CreateCategory;
using DonationHub.Application.Features.Categories.Commands.DeleteCategory;
using DonationHub.Application.Features.Categories.Commands.UpdateCategory;
using DonationHub.Application.Features.Categories.Queries.GetAllCategories;
using DonationHub.Application.Features.Categories.Queries.GetCategoryById;
using DonationHub.Application.Features.Categories.Queries.GetPagedCategories;
using DonationHub.Application.Features.Category.DTOs.Requests;
using DonationHub.Application.Features.Category.DTOs.Responses;
using DonationHub.Application.Features.DTOs.Responses;
using DonationHub.Application.Features.Interfaces;
using DonationHub.Domain.Constants;
using DonationHub.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<CategoryResponseDto>>> GetAll(CancellationToken ct = default)
        {
            var categories = await _mediator.Send(new GetAllCategoriesQuery(), ct);
            return Ok(categories);
        }

        [AllowAnonymous]
        [HttpGet("id")]
        [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryResponseDto>> GetById(Guid id, CancellationToken ct = default)
        {
            var category = await _mediator.Send(new GetCategoryByIdQuery(id), ct);
            return Ok(category);
        }

        [HttpPost]
        [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<CategoryResponseDto>> Create([FromBody] CreateCategoryCommand command, CancellationToken ct = default)
        {
            var category = await _mediator.Send(command);
            return StatusCode(StatusCodes.Status201Created, category);
        }

        [HttpPut("id")]
        [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]

        public async Task<ActionResult<CategoryResponseDto>> Update(Guid id, [FromBody] UpdateCategoryCommand command, CancellationToken ct = default)
        {
            var category = await _mediator.Send(command, ct);

            return Ok(category);
        }
        [HttpDelete("id")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]

        public async Task<ActionResult> Delete(Guid id, CancellationToken ct = default)
        {
            await _mediator.Send(new DeleteCategoryCommand(id), ct);
            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet("paged")]
        [ProducesResponseType(typeof(PagedResponse<CategoryResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<CategoryResponseDto>>> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10,CancellationToken ct = default)
        {
            var category = await _mediator.Send(new GetPagedCategoriesQuery(pageNumber, pageSize), ct);

            return Ok(category);
        }
    }
}
