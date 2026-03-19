using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using GymManagementSystem.Data;
using GymManagementSystem.Models;

namespace GymManagementSystem.Controllers
{
    [Authorize]
    public class FitnessTypesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FitnessTypesController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: FitnessTypes
        public async Task<IActionResult> Index()
        {
            int adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return View(await _context.FitnessTypes.Where(f => f.AdminId == adminId).ToListAsync());
        }

        // GET: FitnessTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var fitnessType = await _context.FitnessTypes
                .Include(f => f.Trainers)
                .Include(f => f.Plans)
                .FirstOrDefaultAsync(m => m.Id == id && m.AdminId == adminId);

            if (fitnessType == null)
            {
                return NotFound();
            }

            return View(fitnessType);
        }

        // GET: FitnessTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FitnessTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] FitnessType fitnessType, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                fitnessType.AdminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                if (imageFile != null && imageFile.Length > 0)
                {
                    fitnessType.ImageUrl = await SaveImageAsync(imageFile, "fitness");
                }

                _context.Add(fitnessType);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Fitness type successfully created!";
                return RedirectToAction(nameof(Index));
            }
            return View(fitnessType);
        }

        // GET: FitnessTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var fitnessType = await _context.FitnessTypes.FirstOrDefaultAsync(f => f.Id == id && f.AdminId == adminId);
            if (fitnessType == null)
            {
                return NotFound();
            }
            return View(fitnessType);
        }

        // POST: FitnessTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,ImageUrl,AdminId")] FitnessType fitnessType, IFormFile? imageFile)
        {
            if (id != fitnessType.Id)
            {
                return NotFound();
            }

            int adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (fitnessType.AdminId != adminId)
            {
                return NotFound(); // Prevent modifying others' data
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        fitnessType.ImageUrl = await SaveImageAsync(imageFile, "fitness");
                    }

                    _context.Update(fitnessType);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Fitness type successfully updated!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FitnessTypeExists(fitnessType.Id))
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
            return View(fitnessType);
        }

        // GET: FitnessTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var fitnessType = await _context.FitnessTypes
                .FirstOrDefaultAsync(m => m.Id == id && m.AdminId == adminId);
            if (fitnessType == null)
            {
                return NotFound();
            }

            return View(fitnessType);
        }

        // POST: FitnessTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var fitnessType = await _context.FitnessTypes.FirstOrDefaultAsync(f => f.Id == id && f.AdminId == adminId);
            if (fitnessType != null)
            {
                // Check for related records
                var hasTrainers = await _context.Trainers.AnyAsync(t => t.FitnessTypes.Any(f => f.Id == id));
                var hasPlans = await _context.Plans.AnyAsync(p => p.FitnessTypeId == id);

                if (hasTrainers || hasPlans)
                {
                    TempData["Error"] = "This fitness type cannot be deleted because it has associated trainers or plans!";
                    return RedirectToAction(nameof(Index));
                }
 
                _context.FitnessTypes.Remove(fitnessType);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Fitness type successfully deleted!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool FitnessTypeExists(int id)
        {
            return _context.FitnessTypes.Any(e => e.Id == id);
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile, string folder)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", folder);
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return $"/images/{folder}/{uniqueFileName}";
        }
    }
}
