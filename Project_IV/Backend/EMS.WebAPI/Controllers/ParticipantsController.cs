using Asp.Versioning;
using EMS.Application.Services.Interfaces;
using EMS.WebAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EMS.WebAPI.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class ParticipantsController : ControllerBase
    {
        private readonly IParticipantService _participantService;

        public ParticipantsController(IParticipantService participantService)
        {
            _participantService = participantService;
        }

        [HttpPost("register/{eventId:guid}")]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> Register(Guid eventId)
        {
            var email = User.FindFirstValue(ClaimTypes.Email)!;
            var result = await _participantService.RegisterForEventAsync(email, eventId);
            if (!result)
                return Conflict(ApiResponse.Fail("Already registered for this event."));

            return Ok(ApiResponse.Ok("Registered successfully."));
        }

        [HttpGet("my-events")]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> MyEvents()
        {
            var email = User.FindFirstValue(ClaimTypes.Email)!;
            var events = await _participantService.GetRegisteredEventsAsync(email);
            return Ok(ApiResponse.Ok(events));
        }

        // ── NEW: Get all registrations for a specific event (Admin only)
        [HttpGet("by-event/{eventId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByEvent(Guid eventId)
        {
            var registrations = await _participantService.GetByEventIdAsync(eventId);
            return Ok(ApiResponse.Ok(registrations));
        }

        // Get a single registration by id.
        // Admin can fetch any registration; Participant can fetch only their own.
        [HttpGet("{registrationId:guid}")]
        [Authorize(Roles = "Admin,Participant")]
        public async Task<IActionResult> GetRegistrationById(Guid registrationId)
        {
            var registration = await _participantService.GetRegistrationByIdAsync(registrationId);
            if (registration == null)
                return NotFound(ApiResponse.Fail("Registration not found."));

            var role = User.FindFirstValue(ClaimTypes.Role)!;
            var email = User.FindFirstValue(ClaimTypes.Email)!;

            // Participants can only view their own registrations.
            if (role == "Participant" && registration.ParticipantEmailId != email)
                return Forbid();

            return Ok(ApiResponse.Ok(registration));
        }

        [HttpGet("by-participant/{email}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByParticipant(string email)
        {
            var registrations = await _participantService.GetRegisteredEventsAsync(email);
            return Ok(ApiResponse.Ok(registrations));
        }

        // Mark attendance for a registration.
        // Admin can mark attendance for anyone; Participant can only mark their own.
        // Ownership enforcement is handled by the service layer.
        [HttpPatch("{registrationId:guid}/attend")]
        [Authorize(Roles = "Admin,Participant")]
        public async Task<IActionResult> MarkAttendance(
            Guid registrationId, [FromQuery] bool attended = true)
        {
            var role = User.FindFirstValue(ClaimTypes.Role)!;
            var email = User.FindFirstValue(ClaimTypes.Email)!;

            // Pass callerEmail for Participant (service enforces ownership);
            // pass null for Admin (service skips ownership check).
            var callerEmail = role == "Participant" ? email : null;

            var result = await _participantService.MarkAttendanceAsync(
                registrationId, attended, callerEmail);

            return result switch
            {
                null => Forbid(),
                false => NotFound(ApiResponse.Fail("Registration not found.")),
                true => Ok(ApiResponse.Ok("Attendance updated."))
            };
        }
    }
}