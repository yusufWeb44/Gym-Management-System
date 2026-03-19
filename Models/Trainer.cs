using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymManagementSystem.Models
{
    public class Trainer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        [Display(Name = "Full Name")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;
 
        [Required(ErrorMessage = "Phone is required")]
        [Display(Name = "Phone")]
        [StringLength(20)]
        public string? Phone { get; set; }
 
        [Required(ErrorMessage = "Email is required")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [RegularExpression(@"^.+@gmail\.com$", ErrorMessage = "Only Gmail addresses are accepted.")]
        [StringLength(100)]
        public string? Email { get; set; }
 
        [Required(ErrorMessage = "Experience is required")]
        [Display(Name = "Experience (Years)")]
        [Range(0, 50)]
        public int ExperienceYears { get; set; }
 
        [Required(ErrorMessage = "Salary is required")]
        [Display(Name = "Salary (₺)")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 1000000)]
        public decimal Salary { get; set; }
 
        [Display(Name = "Profile Picture")]
        [StringLength(255)]
        public string? ImageUrl { get; set; }
 
        // Navigation Properties
        [Display(Name = "Specialties")]
        public virtual ICollection<FitnessType> FitnessTypes { get; set; } = new List<FitnessType>();
        
        public virtual ICollection<Member> Members { get; set; } = new List<Member>();

        // For Multi-tenancy Isolation
        public int AdminId { get; set; }
        [ForeignKey("AdminId")]
        public virtual Admin? Admin { get; set; }
    }
}
