using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManagementSystem.Data;
using GymManagementSystem.Models;
using GymManagementSystem.ViewModels;

namespace GymManagementSystem.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        int adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var today = DateTime.Today;
        var todayPlus3 = today.AddDays(3);

        var viewModel = new DashboardViewModel
        {
            TotalMembers = await _context.Members.CountAsync(m => m.AdminId == adminId && !m.IsDeleted),
            TotalTrainers = await _context.Trainers.CountAsync(t => t.AdminId == adminId),
            TotalPlans = await _context.Plans.CountAsync(p => p.AdminId == adminId),
            TotalFitnessTypes = await _context.FitnessTypes.CountAsync(f => f.AdminId == adminId),
            
            TodayStats = new TodayStatsViewModel
            {
                NewMembersToday = await _context.Members.CountAsync(m => m.AdminId == adminId && !m.IsDeleted && m.RegistrationDate.Date == today),
                StartedMembershipsToday = await _context.Members.CountAsync(m => m.AdminId == adminId && !m.IsDeleted && m.RegistrationDate.Date == today),
                ExpiredMembershipsToday = await _context.Members.Include(m => m.Plan).CountAsync(m => m.AdminId == adminId && !m.IsDeleted && m.RegistrationDate.AddMonths(m.Plan!.DurationMonths).Date == today),
                ActiveMembers = await _context.Members.Include(m => m.Plan).CountAsync(m => m.AdminId == adminId && !m.IsDeleted && m.RegistrationDate.AddMonths(m.Plan!.DurationMonths).Date > today)
            },

            ExpiringMembers = await _context.Members
                .Include(m => m.Plan)
                .Include(m => m.Trainer)
                .Where(m => m.AdminId == adminId 
                         && !m.IsDeleted
                         && m.RegistrationDate.AddMonths(m.Plan!.DurationMonths).Date > today 
                         && m.RegistrationDate.AddMonths(m.Plan!.DurationMonths).Date <= todayPlus3)
                .OrderBy(m => m.RegistrationDate.AddMonths(m.Plan!.DurationMonths))
                .Take(5)
                .ToListAsync(),

            ExpiredMembers = await _context.Members
                .Include(m => m.Plan)
                .Include(m => m.Trainer)
                .Where(m => m.AdminId == adminId 
                         && !m.IsDeleted
                         && m.RegistrationDate.AddMonths(m.Plan!.DurationMonths).Date <= today)
                .OrderByDescending(m => m.RegistrationDate.AddMonths(m.Plan!.DurationMonths))
                .Take(5)
                .ToListAsync(),

            RecentMembers = await _context.Members
                .Where(m => m.AdminId == adminId && !m.IsDeleted)
                .Include(m => m.Plan)
                .Include(m => m.Trainer)
                .OrderByDescending(m => m.RegistrationDate)
                .Take(5)
                .ToListAsync(),
            FitnessTypes = await _context.FitnessTypes
                .Where(f => f.AdminId == adminId)
                .ToListAsync()
        };

        // Get Trainer Workload Data (Active Members Only)
        var trainerLoads = await _context.Trainers
            .Where(t => t.AdminId == adminId)
            .Select(t => new {
                Name = t.FullName,
                Count = _context.Members.Count(m => m.AdminId == adminId 
                                                 && !m.IsDeleted
                                                 && m.TrainerId == t.Id 
                                                 && m.RegistrationDate.AddMonths(m.Plan!.DurationMonths).Date > today)
            })
            .ToListAsync();

        viewModel.TrainerNames = trainerLoads.Select(x => x.Name).ToList();
        viewModel.TrainerMemberCounts = trainerLoads.Select(x => x.Count).ToList();

        // Get Monthly Registrations Data (Last 6 Months)
        var sixMonthsAgo = DateTime.Today.AddMonths(-5);
        sixMonthsAgo = new DateTime(sixMonthsAgo.Year, sixMonthsAgo.Month, 1);

        var monthlyData = await _context.Members
            .Where(m => m.AdminId == adminId && m.RegistrationDate >= sixMonthsAgo)
            .GroupBy(m => new { m.RegistrationDate.Year, m.RegistrationDate.Month })
            .Select(g => new { 
                Year = g.Key.Year, 
                Month = g.Key.Month, 
                Count = g.Count() 
            })
            .ToListAsync();

        // Fill in missing months and format labels
        var labels = new List<string>();
        var counts = new List<int>();
        var culture = new System.Globalization.CultureInfo("en-US");
 
        for (int i = 0; i < 6; i++)
        {
            var date = sixMonthsAgo.AddMonths(i);
            labels.Add(date.ToString("MMM", culture));
            
            var dataPoint = monthlyData.FirstOrDefault(d => d.Year == date.Year && d.Month == date.Month);
            counts.Add(dataPoint?.Count ?? 0);
        }

        viewModel.MonthlyLabels = labels;
        viewModel.MonthlyCounts = counts;

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
