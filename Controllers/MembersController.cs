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
    public class MembersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MembersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Members
        public async Task<IActionResult> Index()
        {
            int adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var members = await _context.Members
                .Where(m => m.AdminId == adminId && !m.IsDeleted)
                .Include(m => m.Plan)
                .Include(m => m.Trainer)
                .ToListAsync();
            return View(members);
        }

        // GET: Members/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var member = await _context.Members
                .Include(m => m.Plan)
                    .ThenInclude(p => p!.FitnessType)
                .Include(m => m.Trainer)
                    .ThenInclude(t => t!.FitnessTypes)
                .FirstOrDefaultAsync(m => m.Id == id && m.AdminId == adminId && !m.IsDeleted);

            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // GET: Members/Create
        public async Task<IActionResult> Create(int? planId)
        {
            int adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var viewModel = new MemberViewModel
            {
                RegistrationDate = DateTime.Now,
                PlanId = planId ?? 0,
                Plans = new SelectList(await _context.Plans.Where(p => p.AdminId == adminId).Include(p => p.FitnessType).ToListAsync(), "Id", "Name", planId)
            };

            if (planId.HasValue && planId.Value > 0)
            {
                var plan = await _context.Plans.FirstOrDefaultAsync(p => p.Id == planId.Value && p.AdminId == adminId);
                if (plan != null)
                {
                    var trainersQuery = _context.Trainers.Where(t => t.AdminId == adminId && t.FitnessTypes.Any(f => f.Id == plan.FitnessTypeId));
                    viewModel.Trainers = new SelectList(await trainersQuery.ToListAsync(), "Id", "FullName");
                }
                else
                {
                    viewModel.Trainers = new SelectList(Enumerable.Empty<SelectListItem>());
                }
            }
            else
            {
                viewModel.Trainers = new SelectList(Enumerable.Empty<SelectListItem>());
            }

            return View(viewModel);
        }

        // POST: Members/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MemberViewModel viewModel)
        {
            int adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            if (ModelState.IsValid)
            {
                var plan = await _context.Plans.FirstOrDefaultAsync(p => p.Id == viewModel.PlanId && p.AdminId == adminId);
                var trainer = await _context.Trainers.Include(t => t.FitnessTypes).FirstOrDefaultAsync(t => t.Id == viewModel.TrainerId && t.AdminId == adminId);

                if (plan != null && trainer != null && !trainer.FitnessTypes.Any(f => f.Id == plan.FitnessTypeId))
                {
                    ModelState.AddModelError("TrainerId", "The selected trainer's specialty does not match the plan's fitness type.");
                }

                if (ModelState.IsValid)
                {
                    var member = new Member
                    {
                        FullName = viewModel.FullName,
                        Email = viewModel.Email,
                        Phone = viewModel.Phone,
                        RegistrationDate = viewModel.RegistrationDate,
                        BirthDate = viewModel.BirthDate!.Value,
                        PlanId = viewModel.PlanId,
                        TrainerId = viewModel.TrainerId,
                        AdminId = adminId
                    };

                    _context.Add(member);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Member successfully created!";
                    return RedirectToAction(nameof(Index));
                }
            }

            viewModel.Plans = new SelectList(_context.Plans.Where(p => p.AdminId == adminId), "Id", "Name", viewModel.PlanId);
            
            var planForDropdown = await _context.Plans.FirstOrDefaultAsync(p => p.Id == viewModel.PlanId && p.AdminId == adminId);
            var trainersQuery = planForDropdown != null 
                ? _context.Trainers.Where(t => t.AdminId == adminId && t.FitnessTypes.Any(f => f.Id == planForDropdown.FitnessTypeId))
                : _context.Trainers.Where(t => t.AdminId == adminId);
                
            viewModel.Trainers = new SelectList(trainersQuery, "Id", "FullName", viewModel.TrainerId);
            return View(viewModel);
        }

        // GET: Members/Edit/5
        public async Task<IActionResult> Edit(int? id, int? planId)
        {
            if (id == null)
            {
                return NotFound();
            }

            int adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var member = await _context.Members.FirstOrDefaultAsync(m => m.Id == id && m.AdminId == adminId && !m.IsDeleted);
            if (member == null)
            {
                return NotFound();
            }

            // If a new planId is provided via the reload button, use that. Otherwise, use the member's saved PlanId.
            int activePlanId = planId ?? member.PlanId;
            var plan = await _context.Plans.FirstOrDefaultAsync(p => p.Id == activePlanId && p.AdminId == adminId);
            
            var trainersQuery = plan != null 
                ? _context.Trainers.Where(t => t.AdminId == adminId && t.FitnessTypes.Any(f => f.Id == plan.FitnessTypeId))
                : _context.Trainers.Where(t => t.AdminId == adminId);

            var viewModel = new MemberViewModel
            {
                Id = member.Id,
                FullName = member.FullName,
                Email = member.Email,
                Phone = member.Phone,
                RegistrationDate = member.RegistrationDate,
                BirthDate = member.BirthDate,
                PlanId = activePlanId,
                TrainerId = planId.HasValue ? 0 : member.TrainerId, // Reset trainer if plan changed during edit
                Plans = new SelectList(await _context.Plans.Where(p => p.AdminId == adminId).ToListAsync(), "Id", "Name", activePlanId),
                Trainers = new SelectList(await trainersQuery.ToListAsync(), "Id", "FullName", planId.HasValue ? null : member.TrainerId)
            };

            return View(viewModel);
        }

        // POST: Members/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MemberViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            int adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            if (ModelState.IsValid)
            {
                var plan = await _context.Plans.FirstOrDefaultAsync(p => p.Id == viewModel.PlanId && p.AdminId == adminId);
                var trainer = await _context.Trainers.Include(t => t.FitnessTypes).FirstOrDefaultAsync(t => t.Id == viewModel.TrainerId && t.AdminId == adminId);

                if (plan != null && trainer != null && !trainer.FitnessTypes.Any(f => f.Id == plan.FitnessTypeId))
                {
                    ModelState.AddModelError("TrainerId", "The selected trainer's specialty does not match the plan's fitness type.");
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        var member = await _context.Members.FirstOrDefaultAsync(m => m.Id == id && m.AdminId == adminId && !m.IsDeleted);
                        if (member == null)
                        {
                            return NotFound();
                        }

                        member.FullName = viewModel.FullName;
                        member.Email = viewModel.Email;
                        member.Phone = viewModel.Phone;
                        member.RegistrationDate = viewModel.RegistrationDate;
                        member.BirthDate = viewModel.BirthDate!.Value;
                        member.PlanId = viewModel.PlanId;
                        member.TrainerId = viewModel.TrainerId;

                        _context.Update(member);
                        await _context.SaveChangesAsync();
                        TempData["Success"] = "Member successfully updated!";
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!MemberExists(viewModel.Id))
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
            }

            viewModel.Plans = new SelectList(_context.Plans.Where(p => p.AdminId == adminId), "Id", "Name", viewModel.PlanId);
            
            var planForDropdown = await _context.Plans.FirstOrDefaultAsync(p => p.Id == viewModel.PlanId && p.AdminId == adminId);
            var trainersQuery = planForDropdown != null 
                ? _context.Trainers.Where(t => t.AdminId == adminId && t.FitnessTypes.Any(f => f.Id == planForDropdown.FitnessTypeId))
                : _context.Trainers.Where(t => t.AdminId == adminId);
                
            viewModel.Trainers = new SelectList(trainersQuery, "Id", "FullName", viewModel.TrainerId);
            return View(viewModel);
        }

        // GET: Members/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var member = await _context.Members
                .Include(m => m.Plan)
                .Include(m => m.Trainer)
                .FirstOrDefaultAsync(m => m.Id == id && m.AdminId == adminId && !m.IsDeleted);

            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // POST: Members/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var member = await _context.Members.FirstOrDefaultAsync(m => m.Id == id && m.AdminId == adminId && !m.IsDeleted);
            if (member != null)
            {
                member.IsDeleted = true;
                _context.Update(member);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Member successfully deleted!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool MemberExists(int id)
        {
            return _context.Members.Any(e => e.Id == id);
        }
    }
}
