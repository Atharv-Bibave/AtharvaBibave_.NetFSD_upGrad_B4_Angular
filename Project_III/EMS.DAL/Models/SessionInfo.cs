using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagementSystem.DataAccessLayer.Models
{
    public class SessionInfo
    {
        [Key]
        public Guid SessionId { get; set; }

        [Required]
        public Guid EventId { get; set; }   

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string SessionTitle { get; set; } = string.Empty;

        public Guid? SpeakerId { get; set; }  

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public DateTime SessionStart { get; set; }

        [Required]
        public DateTime SessionEnd { get; set; }

        [StringLength(2048)]
        public string? SessionUrl { get; set; }

        [ForeignKey("EventId")]
        public EventDetails? Event { get; set; }

        [ForeignKey("SpeakerId")]
        public SpeakersDetails? Speaker { get; set; }
    }
}
