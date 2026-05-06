using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.AppUI.Models
{
    public class SpeakerViewModel
    {
        public Guid SpeakerId { get; set; }

        [Required(ErrorMessage = "Speaker name is required.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Name must be 1–50 characters.")]
        [Display(Name = "Speaker Name")]
        public string SpeakerName { get; set; } = string.Empty;
    }
}
