using System.ComponentModel.DataAnnotations;
using GymManagementSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagementSystem.ViewModels
{
    public class TrainerViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        [Display(Name = "Full Name")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Phone")]
        [StringLength(20)]
        public string? Phone { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [RegularExpression(@"^.+@gmail\.com$", ErrorMessage = "Only Gmail addresses are accepted.")]
        [StringLength(100)]
        public string? Email { get; set; }

        [Display(Name = "Experience (Years)")]
        [Range(0, 50)]
        public int ExperienceYears { get; set; }

        [Required(ErrorMessage = "At least one specialty must be selected")]
        [Display(Name = "Specialties")]
        public List<int> SelectedFitnessTypeIds { get; set; } = new List<int>();

        [Required(ErrorMessage = "Salary is required")]
        [Display(Name = "Salary (₺)")]
        [Range(0, 1000000, ErrorMessage = "Salary must be between 0 and 1,000,000")]
        public decimal Salary { get; set; }

        [Display(Name = "Upload Photo")]
        [DataType(DataType.Upload)]
        public IFormFile? Image { get; set; }

        public SelectList? FitnessTypes { get; set; }
    }
}
