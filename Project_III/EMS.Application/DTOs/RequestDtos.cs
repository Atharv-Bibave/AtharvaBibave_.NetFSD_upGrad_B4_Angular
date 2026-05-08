using System.ComponentModel.DataAnnotations;
using EMS.Application.Validation;

namespace EMS.Application.DTOs
{
    // ── Auth / User 

    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string EmailId { get; set; } = string.Empty;

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [StringLength(20, MinimumLength = 6)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[^a-zA-Z0-9]).+$",
            ErrorMessage = "Password must have at least one uppercase letter and one special character.")]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string EmailId { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    // ── Event

    public class EventDto
    {
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string EventName { get; set; } = string.Empty;

        [Required]
        public Guid CategoryId { get; set; }

        [Required]
        [FutureDate]
        public DateTime EventDate { get; set; }

        public string? Description { get; set; }

        [RegularExpression("^(Active|In-Active)$", ErrorMessage = "Status must be Active or In-Active.")]
        public string Status { get; set; } = "Active";
    }

    // ── Session

    [SessionDateRange(nameof(SessionStart), nameof(SessionEnd))]
    public class SessionDto
    {
        [Required]
        public Guid EventId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string SessionTitle { get; set; } = string.Empty;

        public Guid? SpeakerId { get; set; }

        public string? Description { get; set; }

        [Required]
        public DateTime SessionStart { get; set; }

        [Required]
        public DateTime SessionEnd { get; set; }

        [Url]
        public string? SessionUrl { get; set; }
    }

    // ── Speaker 
    public class SpeakerDto
    {
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string SpeakerName { get; set; } = string.Empty;
    }

    // ── Category 

    public class CategoryDto
    {
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string CategoryName { get; set; } = string.Empty;
    }
}
