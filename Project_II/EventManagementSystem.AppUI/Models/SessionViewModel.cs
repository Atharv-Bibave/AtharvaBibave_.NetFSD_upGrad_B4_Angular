using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.AppUI.Models
{
    public class SessionViewModel
    {
        public Guid SessionId { get; set; }

        [Required(ErrorMessage = "Session title is required.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Title must be 1–50 characters.")]
        [Display(Name = "Session Title")]
        public string SessionTitle { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Session URL")]
        [Url(ErrorMessage = "Please enter a valid URL (e.g. https://zoom.us/j/...).")]
        public string? SessionUrl { get; set; }

        [Required(ErrorMessage = "Please select an event.")]
        [Display(Name = "Event")]
        public Guid EventId { get; set; }

        [Display(Name = "Speaker")]
        public Guid? SpeakerId { get; set; }

        [Required(ErrorMessage = "Start time is required.")]
        [Display(Name = "Start Time")]
        public DateTime SessionStart { get; set; }

        [Required(ErrorMessage = "End time is required.")]
        [Display(Name = "End Time")]
        public DateTime SessionEnd { get; set; }
    }
}
