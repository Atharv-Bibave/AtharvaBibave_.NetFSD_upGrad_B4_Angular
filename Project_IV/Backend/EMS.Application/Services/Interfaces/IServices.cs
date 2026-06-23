using EMS.Application.DTOs;

namespace EMS.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> RegisterAsync(RegisterDto dto);
        Task<string?> LoginAsync(LoginDto dto);
        Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
    }

    public interface IEventService
    {
        Task<PagedResult<EventResponseDto>> GetAllAsync(int page, int pageSize);
        Task<EventResponseDto?> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(EventDto dto);
        Task<bool> UpdateAsync(Guid id, EventDto dto);
        Task<bool> DeleteAsync(Guid id);
    }

    public interface ISessionService
    {
        Task<PagedResult<SessionResponseDto>> GetByEventAsync(Guid eventId, int page, int pageSize);
        Task<SessionResponseDto?> GetByIdAsync(Guid id);
        Task<bool> AddAsync(SessionDto dto);
        Task<bool> UpdateAsync(Guid id, SessionDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> AssignSpeakerAsync(Guid sessionId, Guid speakerId);
    }

    public interface ISpeakerService
    {
        Task<IEnumerable<SpeakerResponseDto>> GetAllAsync();
        Task<SpeakerResponseDto?> GetByIdAsync(Guid id);
        Task<bool> AddAsync(SpeakerDto dto);
        Task<bool> DeleteAsync(Guid id);
    }

    public interface IParticipantService
    {
        Task<bool> RegisterForEventAsync(string email, Guid eventId);
        Task<IEnumerable<ParticipantRegistrationResponseDto>> GetRegisteredEventsAsync(string email);
        // Mark attendance. If callerEmail is non-null (Participant role), the service
        // enforces that the registration belongs to that participant; returns null on
        // ownership violation so the controller can return 403.
        // Admin passes callerEmail = null to bypass the ownership check.
        Task<bool?> MarkAttendanceAsync(Guid registrationId, bool attended, string? callerEmail);
        Task<ParticipantRegistrationResponseDto?> GetRegistrationByIdAsync(Guid registrationId);
        Task<IEnumerable<ParticipantRegistrationResponseDto>> GetByEventIdAsync(Guid eventId);
    }

    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseDto>> GetAllAsync();
        Task<bool> AddAsync(CategoryDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
