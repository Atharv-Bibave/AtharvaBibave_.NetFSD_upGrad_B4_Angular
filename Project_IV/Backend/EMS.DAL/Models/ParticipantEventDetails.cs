using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagementSystem.DataAccessLayer.Models
{
    public class ParticipantEventDetails
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(450)]
        public string ParticipantEmailId { get; set; } = string.Empty;

        [Required]
        public Guid EventId { get; set; }

        public bool IsAttended { get; set; }

        [ForeignKey("ParticipantEmailId")]
        public UserInfo? User { get; set; }

        [ForeignKey("EventId")]
        public EventDetails? Event { get; set; }
    }
}