using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GymManagementSystem.Attributes;

namespace GymManagementSystem.Models
{
    public class Member
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        [Display(Name = "Full Name")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;
 
        [Required(ErrorMessage = "Email is required")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [RegularExpression(@"^.+@gmail\.com$", ErrorMessage = "Only Gmail addresses are accepted.")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone is required")]
        [Display(Name = "Phone")]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;



        [Display(Name = "Registration Date")]
        [DataType(DataType.Date)]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Birth date is required")]
        [Display(Name = "Birth Date")]
        [DataType(DataType.Date)]
        [GymManagementSystem.Attributes.MinimumAge(16, ErrorMessage = "Minimum age must be 16")]
        public DateTime BirthDate { get; set; }

        // Foreign Keys
        [Required(ErrorMessage = "Plan selection is required")]
        [Display(Name = "Membership Plan")]
        public int PlanId { get; set; }
 
        [Required(ErrorMessage = "Trainer selection is required")]
        [Display(Name = "Trainer")]
        public int TrainerId { get; set; }

        // Navigation Properties
        [ForeignKey("PlanId")]
        public virtual Plan? Plan { get; set; }

        [ForeignKey("TrainerId")]
        public virtual Trainer? Trainer { get; set; }

        // For Multi-tenancy Isolation
        public int AdminId { get; set; }
        [ForeignKey("AdminId")]
        public virtual Admin? Admin { get; set; }

        public bool IsDeleted { get; set; } = false;

        [NotMapped]
        public int RemainingDays
        {
            get
            {
                if (Plan == null) return 0;
                var expiryDate = RegistrationDate.AddMonths(Plan.DurationMonths).Date;
                var today = DateTime.Today;
                var remaining = (expiryDate - today).Days;
                return remaining > 0 ? remaining : 0;
            }
        }

        [NotMapped]
        public string RemainingTimeText
        {
            get
            {
                if (Plan == null) return "No Plan";
                
                var today = DateTime.Today;
                var expiryDate = RegistrationDate.AddMonths(Plan.DurationMonths).Date;
 
                if (today >= expiryDate) return "Expired";

                // Year/Month/Day calculation
                int months = 0;
                var tempDate = RegistrationDate.Date;
                
                // If registration is today or in future, count from today
                var startDate = today > RegistrationDate.Date ? today : RegistrationDate.Date;
                tempDate = startDate;

                while (tempDate.AddMonths(1) <= expiryDate)
                {
                    tempDate = tempDate.AddMonths(1);
                    months++;
                }

                int days = (expiryDate - tempDate).Days;
 
                if (months > 0 && days > 0)
                    return $"{months} Month(s) {days} Day(s)";
                if (months > 0)
                    return $"{months} Month(s)";
                
                return $"{days} Day(s) Left";
            }
        }
    }
}
