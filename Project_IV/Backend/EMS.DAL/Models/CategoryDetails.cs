using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.DataAccessLayer.Models
{
    public class CategoryDetails
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string CategoryName { get; set; } = string.Empty;
    }
}
