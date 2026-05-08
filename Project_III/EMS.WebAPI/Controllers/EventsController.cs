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
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _eventService.GetAllAsync(page, pageSize);
            return Ok(ApiResponse.Ok(result));
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var ev = await _eventService.GetByIdAsync(id);
            if (ev == null)
                return NotFound(ApiResponse.Fail("Event not found."));

            return Ok(ApiResponse.Ok(ev));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] EventDto dto)
        {
            var result = await _eventService.CreateAsync(dto);
            if (!result)
                return BadRequest(ApiResponse.Fail("Could not create event. The category ID may be invalid."));

            return StatusCode(201, ApiResponse.Created("Event created."));
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] EventDto dto)
        {
            var result = await _eventService.UpdateAsync(id, dto);
            if (!result)
                return NotFound(ApiResponse.Fail("Event not found or category ID is invalid."));

            return Ok(ApiResponse.Ok("Event updated."));
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _eventService.DeleteAsync(id);
            if (!result)
                return NotFound(ApiResponse.Fail("Event not found."));

            return Ok(ApiResponse.Ok("Event deleted."));
        }
    }
}
