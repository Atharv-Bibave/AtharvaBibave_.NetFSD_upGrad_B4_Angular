using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.DataAccessLayer.Models
{
    public class SpeakersDetails
    {
        [Key]
        public Guid SpeakerId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string SpeakerName { get; set; } = string.Empty;
    }
}
