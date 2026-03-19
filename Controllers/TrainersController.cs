using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using GymManagementSystem.Data;
using GymManagementSystem.Models;
using GymManagementSystem.ViewModels;

namespace GymManagementSystem.Controllers
{
    [Authorize]
    public class TrainersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TrainersController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Trainers
        public async Task<IActionResult> Index()
        {
            int adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var trainers = await _context.Trainers
                .Include(t => t.FitnessTypes)
                .Where(t => t.AdminId == adminId)
                .ToListAsync();
            return View(trainers);
        }

        // GET: Trainers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var trainer = await _context.Trainers
                .Include(t => t.FitnessTypes)
                .Include(t => t.Members.Where(m => !m.IsDeleted))
                .FirstOrDefaultAsync(m => m.Id == id && m.AdminId == adminId);

            if (trainer == null)
            {
                return NotFound();
            }

            return View(trainer);
        }

        // GET: Trainers/Create
        public IActionResult Create()
        {
            int adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var viewModel = new TrainerViewModel
            {
                FitnessTypes = new SelectList(_context.FitnessTypes.Where(f => f.AdminId == adminId), "Id", "Name")
            };
            return View(viewModel);
        }

        // POST: Trainers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrainerViewModel viewModel)
        {
            int adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            if (ModelState.IsValid)
            {
                var trainer = new Trainer
                {
                    FullName = viewModel.FullName,
                    Phone = viewModel.Phone,
                    Email = viewModel.Email,
                    ExperienceYears = viewModel.ExperienceYears,
                    Salary = viewModel.Salary,
                    AdminId = adminId
                };

                if (viewModel.SelectedFitnessTypeIds != null && viewModel.SelectedFitnessTypeIds.Any())
                {
                    trainer.FitnessTypes = await _context.FitnessTypes
                        .Where(f => viewModel.SelectedFitnessTypeIds.Contains(f.Id) && f.AdminId == adminId)
                        .ToListAsync();
                }

                if (viewModel.Image != null && viewModel.Image.Length > 0)
                {
                    trainer.ImageUrl = await SaveImageAsync(viewModel.Image);
                }

                _context.Add(trainer);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Trainer successfully created!";
                return RedirectToAction(nameof(Index));
            }
            viewModel.FitnessTypes = new SelectList(_context.FitnessTypes.Where(f => f.AdminId == adminId), "Id", "Name");
            return View(viewModel);
        }

        // GET: Trainers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var trainer = await _context.Trainers
                .Include(t => t.FitnessTypes)
                .FirstOrDefaultAsync(t => t.Id == id && t.AdminId == adminId);

            if (trainer == null)
            {
                return NotFound();
            }

            var viewModel = new TrainerViewModel
            {
                Id = trainer.Id,
                FullName = trainer.FullName,
                Phone = trainer.Phone,
                Email = trainer.Email,
                ExperienceYears = trainer.ExperienceYears,
                SelectedFitnessTypeIds = trainer.FitnessTypes.Select(f => f.Id).ToList(),
                Salary = trainer.Salary,
                FitnessTypes = new SelectList(_context.FitnessTypes.Where(f => f.AdminId == adminId), "Id", "Name")
            };
            
            ViewBag.CurrentImageUrl = trainer.ImageUrl;

            return View(viewModel);
        }

        // POST: Trainers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TrainerViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            int adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            if (ModelState.IsValid)
            {
                try
                {
                    var trainer = await _context.Trainers
                        .Include(t => t.FitnessTypes)
                        .FirstOrDefaultAsync(t => t.Id == id && t.AdminId == adminId);

                    if (trainer == null)
                    {
                        return NotFound();
                    }

                    trainer.FullName = viewModel.FullName;
                    trainer.Phone = viewModel.Phone;
                    trainer.Email = viewModel.Email;
                    trainer.ExperienceYears = viewModel.ExperienceYears;
                    trainer.Salary = viewModel.Salary;

                    // Update many-to-many relationship
                    trainer.FitnessTypes.Clear();
                    if (viewModel.SelectedFitnessTypeIds != null && viewModel.SelectedFitnessTypeIds.Any())
                    {
                        trainer.FitnessTypes = await _context.FitnessTypes
                            .Where(f => viewModel.SelectedFitnessTypeIds.Contains(f.Id) && f.AdminId == adminId)
                            .ToListAsync();
                    }

                    if (viewModel.Image != null && viewModel.Image.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(trainer.ImageUrl))
                        {
                            var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, trainer.ImageUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldPath))
                            {
                                System.IO.File.Delete(oldPath);
                            }
                        }

                        trainer.ImageUrl = await SaveImageAsync(viewModel.Image);
                    }

                    _context.Update(trainer);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Trainer successfully updated!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainerExists(viewModel.Id))
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
            viewModel.FitnessTypes = new SelectList(_context.FitnessTypes.Where(f => f.AdminId == adminId), "Id", "Name");
             var existingTrainer = await _context.Trainers.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id && t.AdminId == adminId);
             if (existingTrainer != null) ViewBag.CurrentImageUrl = existingTrainer.ImageUrl;

            return View(viewModel);
        }

        // GET: Trainers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var trainer = await _context.Trainers
                .Include(t => t.FitnessTypes)
                .FirstOrDefaultAsync(m => m.Id == id && m.AdminId == adminId);

            if (trainer == null)
            {
                return NotFound();
            }

            return View(trainer);
        }

        // POST: Trainers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var trainer = await _context.Trainers.FirstOrDefaultAsync(t => t.Id == id && t.AdminId == adminId);
            if (trainer != null)
            {
                var hasMembers = await _context.Members.AnyAsync(m => m.TrainerId == id && m.AdminId == adminId && !m.IsDeleted);
                if (hasMembers)
                {
                    TempData["Error"] = "This trainer cannot be deleted because they have associated members!";
                    return RedirectToAction(nameof(Index));
                }
                
                // Delete image if exists
                if (!string.IsNullOrEmpty(trainer.ImageUrl))
                {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, trainer.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                         System.IO.File.Delete(imagePath);
                    }
                }

                _context.Trainers.Remove(trainer);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Trainer successfully deleted!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TrainerExists(int id)
        {
            return _context.Trainers.Any(e => e.Id == id);
        }

        [HttpGet]
        public async Task<IActionResult> GetTrainersByPlan(int planId)
        {
            int adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var plan = await _context.Plans.FirstOrDefaultAsync(p => p.Id == planId && p.AdminId == adminId);
            if (plan == null)
            {
                return Json(new List<object>());
            }

            var trainers = await _context.Trainers
                .Where(t => t.AdminId == adminId && t.FitnessTypes.Any(f => f.Id == plan.FitnessTypeId))
                .Select(t => new { id = t.Id, fullName = t.FullName })
                .ToListAsync();

            return Json(trainers);
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "trainers");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return $"/images/trainers/{uniqueFileName}";
        }
    }
}
