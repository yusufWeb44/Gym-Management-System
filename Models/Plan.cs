using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymManagementSystem.Models
{
    public class Plan
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Plan name is required")]
        [Display(Name = "Plan Name")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
 
        [Required(ErrorMessage = "Price is required")]
        [Display(Name = "Total Price (₺)")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 100000)]
        public decimal Price { get; set; }
 
        [Required(ErrorMessage = "Duration is required")]
        [Display(Name = "Duration (Months)")]
        [Range(1, 36)]
        public int DurationMonths { get; set; }
 
        [Display(Name = "Description")]
        [StringLength(500)]
        public string? Description { get; set; }
 
        // Foreign Key
        [Required(ErrorMessage = "Fitness type selection is required")]
        [Display(Name = "Fitness Type")]
        public int FitnessTypeId { get; set; }

        // Navigation Properties
        [ForeignKey("FitnessTypeId")]
        public virtual FitnessType? FitnessType { get; set; }
        
        public virtual ICollection<Member> Members { get; set; } = new List<Member>();

        // For Multi-tenancy Isolation
        public int AdminId { get; set; }
        [ForeignKey("AdminId")]
        public virtual Admin? Admin { get; set; }
    }
}
