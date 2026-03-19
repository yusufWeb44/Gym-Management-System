using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymManagementSystem.Models
{
    public class FitnessType
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name field is required")]
        [Display(Name = "Fitness Type")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
 
        [Display(Name = "Description")]
        [StringLength(500)]
        public string? Description { get; set; }
 
        [Display(Name = "Image URL")]
        [StringLength(255)]
        public string? ImageUrl { get; set; }

        // For Multi-tenancy Isolation
        public int AdminId { get; set; }
        [ForeignKey("AdminId")]
        public virtual Admin? Admin { get; set; }

        // Navigation Properties
        public virtual ICollection<Trainer> Trainers { get; set; } = new List<Trainer>();
        public virtual ICollection<Plan> Plans { get; set; } = new List<Plan>();
    }
}
