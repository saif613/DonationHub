using Asp.Versioning;
using DonationHub.Application.DTOs.Requests;
using DonationHub.Application.DTOs.Responses;
using DonationHub.Application.Features.Interfaces;
using DonationHub.Domain.Constants;
using DonationHub.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DonationHub.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Admin)]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<CategoryResponseDto>>> GetAll(CancellationToken ct = default)
        {
            var categories = await _categoryService.GetAllAsync(ct);
            return Ok(categories);
        }

        [AllowAnonymous]
        [HttpGet("id")]
        [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryResponseDto>> GetById(Guid id, CancellationToken ct = default)
        {
            var category = await _categoryService.GetByIdAsync(id, ct);
            return Ok(category);
        }

        [HttpPost]
        [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<CategoryResponseDto>> Create([FromBody] CategoryRequestDto requestDto, CancellationToken ct = default)
        {
            var category = await _categoryService.CreateAsync(requestDto, ct);
            return StatusCode(StatusCodes.Status201Created, category);
        }

        [HttpPut("id")]
        [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]

        public async Task<ActionResult<CategoryResponseDto>> Update([FromBody] CategoryRequestDto requestDto, Guid id, CancellationToken ct = default)
        {
            var category = await _categoryService.Update(requestDto, id, ct);

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
            await _categoryService.SoftDelete(id, ct);
            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet("paged")]
        [ProducesResponseType(typeof(PagedResponse<CategoryResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<CategoryResponseDto>>> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var category = await _categoryService.PagedAsync(pageNumber, pageSize, ct);

            return Ok(category);
        }
    }
}
