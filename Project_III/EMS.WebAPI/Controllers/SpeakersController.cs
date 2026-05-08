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
    [Authorize]
    public class SpeakersController : ControllerBase
    {
        private readonly ISpeakerService _speakerService;

        public SpeakersController(ISpeakerService speakerService)
        {
            _speakerService = speakerService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var speakers = await _speakerService.GetAllAsync();
            return Ok(ApiResponse.Ok(speakers));
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var speaker = await _speakerService.GetByIdAsync(id);
            if (speaker == null)
                return NotFound(ApiResponse.Fail("Speaker not found."));

            return Ok(ApiResponse.Ok(speaker));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add([FromBody] SpeakerDto dto)
        {
            var result = await _speakerService.AddAsync(dto);
            if (!result)
                return BadRequest(ApiResponse.Fail("Could not add speaker."));

            return StatusCode(201, ApiResponse.Created("Speaker added."));
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _speakerService.DeleteAsync(id);
            if (!result)
                return NotFound(ApiResponse.Fail("Speaker not found."));

            return Ok(ApiResponse.Ok("Speaker deleted."));
        }
    }
}
