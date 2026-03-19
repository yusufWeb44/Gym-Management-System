using GymManagementSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using GymManagementSystem.Attributes;

namespace GymManagementSystem.ViewModels
{
    public class MemberViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        [Display(Name = "Full Name")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [RegularExpression(@"^.+@gmail\.com$", ErrorMessage = "Only Gmail addresses are accepted.")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone is required")]
        [Display(Name = "Phone")]
        [StringLength(20)]
        public string? Phone { get; set; }



        [Display(Name = "Registration Date")]
        [DataType(DataType.Date)]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Birth date is required")]
        [Display(Name = "Birth Date")]
        [DataType(DataType.Date)]
        [GymManagementSystem.Attributes.MinimumAge(16, ErrorMessage = "Minimum age must be 16")]
        public DateTime? BirthDate { get; set; }

        [Required(ErrorMessage = "Plan selection is required")]
        [Display(Name = "Membership Plan")]
        public int PlanId { get; set; }
 
        [Required(ErrorMessage = "Trainer selection is required")]
        [Display(Name = "Trainer")]
        public int TrainerId { get; set; }

        public SelectList? Plans { get; set; }
        public SelectList? Trainers { get; set; }
    }
}
