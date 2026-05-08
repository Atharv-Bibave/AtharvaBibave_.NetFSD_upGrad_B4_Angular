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
    public class SessionsController : ControllerBase
    {
        private readonly ISessionService _sessionService;

        public SessionsController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        [HttpGet("by-event/{eventId:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByEvent(
            Guid eventId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _sessionService.GetByEventAsync(eventId, page, pageSize);
            return Ok(ApiResponse.Ok(result));
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var session = await _sessionService.GetByIdAsync(id);
            if (session == null)
                return NotFound(ApiResponse.Fail("Session not found."));

            return Ok(ApiResponse.Ok(session));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add([FromBody] SessionDto dto)
        {
            if (dto.SessionEnd <= dto.SessionStart)
                return BadRequest(ApiResponse.Fail("SessionEnd must be after SessionStart."));

            var result = await _sessionService.AddAsync(dto);
            if (!result)
                return BadRequest(ApiResponse.Fail("Could not add session. Verify EventId exists and SessionStart is on or after the event date."));

            return StatusCode(201, ApiResponse.Created("Session added successfully."));
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SessionDto dto)
        {
            var result = await _sessionService.UpdateAsync(id, dto);
            if (!result)
                return NotFound(ApiResponse.Fail("Session not found."));

            return Ok(ApiResponse.Ok("Session updated."));
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _sessionService.DeleteAsync(id);
            if (!result)
                return NotFound(ApiResponse.Fail("Session not found."));

            return Ok(ApiResponse.Ok("Session deleted."));
        }

        [HttpPatch("{id:guid}/assign-speaker/{speakerId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignSpeaker(Guid id, Guid speakerId)
        {
            var result = await _sessionService.AssignSpeakerAsync(id, speakerId);
            if (!result)
                return NotFound(ApiResponse.Fail("Session or speaker not found."));

            return Ok(ApiResponse.Ok("Speaker assigned."));
        }
    }
}
