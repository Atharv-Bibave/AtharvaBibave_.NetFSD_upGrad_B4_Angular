namespace EMS.Application.DTOs
{
    // ── Paged result wrapper 

    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = [];
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }

    // ── Event 

    public class EventResponseDto
    {
        public Guid EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string EventCategory { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public DateTime EventDate { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    // ── Session 

    public class SessionResponseDto
    {
        public Guid SessionId { get; set; }
        public Guid EventId { get; set; }
        public string SessionTitle { get; set; } = string.Empty;
        public Guid? SpeakerId { get; set; }
        public string? SpeakerName { get; set; }
        public string? Description { get; set; }
        public DateTime SessionStart { get; set; }
        public DateTime SessionEnd { get; set; }
        public string? SessionUrl { get; set; }
    }

    // ── Speaker

    public class SpeakerResponseDto
    {
        public Guid SpeakerId { get; set; }
        public string SpeakerName { get; set; } = string.Empty;
    }

    // ── User

    public class UserResponseDto
    {
        public string EmailId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    // ── Participant

    public class ParticipantRegistrationResponseDto
    {
        public Guid Id { get; set; }
        public string ParticipantEmailId { get; set; } = string.Empty;
        public Guid EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public bool IsAttended { get; set; }
    }

    // ── Category 

    public class CategoryResponseDto
    {
        public Guid Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
