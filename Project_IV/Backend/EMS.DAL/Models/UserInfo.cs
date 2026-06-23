using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.DataAccessLayer.Models
{
    public class UserInfo
    {
        [Key]
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [MaxLength(450)]
        public string EmailId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 50 characters.")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(Admin|Participant)$", ErrorMessage = "Role must be 'Admin' or 'Participant'.")]
        public string Role { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}