using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.Models
{
    public class Admin
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(100)]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [Display(Name = "Password")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Role")]
        public string Role { get; set; } = "Admin";

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
 
        [Display(Name = "Status")]
        public bool IsActive { get; set; } = true;
    }
}
