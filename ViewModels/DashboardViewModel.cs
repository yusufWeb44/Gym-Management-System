using GymManagementSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.ViewModels
{
    public class TodayStatsViewModel
    {
        [Display(Name = "New Members (Today)")]
        public int NewMembersToday { get; set; }
 
        [Display(Name = "Started Memberships (Today)")]
        public int StartedMembershipsToday { get; set; }
 
        [Display(Name = "Expired Memberships (Today)")]
        public int ExpiredMembershipsToday { get; set; }
 
        [Display(Name = "Active Members")]
        public int ActiveMembers { get; set; }
    }

    public class DashboardViewModel
    {
        public int TotalMembers { get; set; }
        public int TotalTrainers { get; set; }
        public int TotalPlans { get; set; }
        public int TotalFitnessTypes { get; set; }
        
        public TodayStatsViewModel TodayStats { get; set; } = new TodayStatsViewModel();
        public List<Member> ExpiringMembers { get; set; } = new List<Member>();
        public List<Member> ExpiredMembers { get; set; } = new List<Member>();

        public List<Member> RecentMembers { get; set; } = new List<Member>();
        public List<FitnessType> FitnessTypes { get; set; } = new List<FitnessType>();
        
        // Trainer Workload Chart Data
        public List<string> TrainerNames { get; set; } = new List<string>();
        public List<int> TrainerMemberCounts { get; set; } = new List<int>();

        // Monthly Registrations Chart Data
        public List<string> MonthlyLabels { get; set; } = new List<string>();
        public List<int> MonthlyCounts { get; set; } = new List<int>();
    }
}
