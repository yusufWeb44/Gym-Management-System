using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using GymManagementSystem.Data;
using GymManagementSystem.Models;

namespace GymManagementSystem.Controllers
{
    [Authorize]
    public class PlansController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PlansController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Plans
        public async Task<IActionResult> Index()
        {
            int adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var plans = await _context.Plans
                .Include(p => p.FitnessType)
                .Where(p => p.AdminId == adminId)
                .ToListAsync();
            return View(plans);
        }

        // GET: Plans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var plan = await _context.Plans
                .Include(p => p.FitnessType)
                .Include(p => p.Members.Where(m => !m.IsDeleted))
                .FirstOrDefaultAsync(m => m.Id == id && m.AdminId == adminId);

            if (plan == null)
            {
                return NotFound();
            }

            return View(plan);
        }

        // GET: Plans/Create
        public IActionResult Create()
        {
            int adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            ViewData["FitnessTypeId"] = new SelectList(_context.FitnessTypes.Where(f => f.AdminId == adminId), "Id", "Name");
            return View();
        }

        // POST: Plans/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,DurationMonths,Description,FitnessTypeId")] Plan plan)
        {
            int adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            if (ModelState.IsValid)
            {
                plan.AdminId = adminId;
                _context.Add(plan);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Membership plan successfully created!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["FitnessTypeId"] = new SelectList(_context.FitnessTypes.Where(f => f.AdminId == adminId), "Id", "Name", plan.FitnessTypeId);
            return View(plan);
        }

        // GET: Plans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var plan = await _context.Plans.FirstOrDefaultAsync(p => p.Id == id && p.AdminId == adminId);
            if (plan == null)
            {
                return NotFound();
            }
            ViewData["FitnessTypeId"] = new SelectList(_context.FitnessTypes.Where(f => f.AdminId == adminId), "Id", "Name", plan.FitnessTypeId);
            return View(plan);
        }

        // POST: Plans/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,DurationMonths,Description,FitnessTypeId,AdminId")] Plan plan)
        {
            if (id != plan.Id)
            {
                return NotFound();
            }

            int adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (plan.AdminId != adminId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(plan);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Membership plan successfully updated!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlanExists(plan.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["FitnessTypeId"] = new SelectList(_context.FitnessTypes.Where(f => f.AdminId == adminId), "Id", "Name", plan.FitnessTypeId);
            return View(plan);
        }

        // GET: Plans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var plan = await _context.Plans
                .Include(p => p.FitnessType)
                .FirstOrDefaultAsync(m => m.Id == id && m.AdminId == adminId);

            if (plan == null)
            {
                return NotFound();
            }

            return View(plan);
        }

        // POST: Plans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var plan = await _context.Plans.FirstOrDefaultAsync(p => p.Id == id && p.AdminId == adminId);
            
            if (plan != null)
            {
                var hasMembers = await _context.Members.AnyAsync(m => m.PlanId == id && m.AdminId == adminId && !m.IsDeleted);
                if (hasMembers)
                {
                    TempData["Error"] = "This plan cannot be deleted because it has registered members!";
                    return RedirectToAction(nameof(Index));
                }
 
                _context.Plans.Remove(plan);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Membership plan successfully deleted!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PlanExists(int id)
        {
            return _context.Plans.Any(e => e.Id == id);
        }
    }
}
