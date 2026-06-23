using Asp.Versioning;
using EMS.Application.DTOs;
using EMS.Application.Services.Interfaces;
using EMS.WebAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMS.WebAPI.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(ApiResponse.Ok(categories));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add([FromBody] CategoryDto dto)
        {
            var result = await _categoryService.AddAsync(dto);
            if (!result)
                return BadRequest(ApiResponse.Fail("Could not add category. Name may already exist."));

            return StatusCode(201, ApiResponse.Created("Category added."));
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _categoryService.DeleteAsync(id);
            if (!result)
                return NotFound(ApiResponse.Fail("Category not found."));

            return Ok(ApiResponse.Ok("Category deleted."));
        }
    }
}
