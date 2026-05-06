using System.ComponentModel.DataAnnotations;
//using EventManagementSystem.DataAccessLayer.Validators;

namespace EventManagementSystem.AppUI.Models
{
    public class EventViewModel
    {
        public Guid EventId { get; set; }

        public bool IsEditMode { get; set; } = false;

        [Required(ErrorMessage = "Event name is required.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Name must be 1-50 characters.")]
        [Display(Name = "Event Name")]
        public string EventName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Category must be 1-50 characters.")]
        [Display(Name = "Category")]
        public string EventCategory { get; set; } = string.Empty;

        [Required(ErrorMessage = "Event date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Event Date")]
        public DateTime EventDate { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Active";
    }
}
