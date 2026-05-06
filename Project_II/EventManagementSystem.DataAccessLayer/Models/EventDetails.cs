using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.DataAccessLayer.Models
{
    public class EventDetails
    {
        [Key]
        public Guid EventId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string EventName { get; set; } = string.Empty;

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string EventCategory { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [RegularExpression("^(Active|In-Active)$", ErrorMessage = "Status must be Active or In-Active.")]
        [StringLength(20)]
        public string Status { get; set; } = "Active";

        public ICollection<SessionInfo> Sessions { get; set; } = new List<SessionInfo>();
    }
}
