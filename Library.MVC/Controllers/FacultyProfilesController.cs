using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Library.Domain;
using Library.MVC.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Library.MVC.Controllers
{
    [Authorize(Roles = "Faculty,Admin")]
    public class FacultyProfilesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public FacultyProfilesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: FacultyProfiles
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Faculty"))
            {
                // Redirect faculty to their own details page
                var currentUser = await _userManager.GetUserAsync(User);
                var facultyProfile = await _context.FacultyProfiles
                    .FirstOrDefaultAsync(f => f.IdentityUserId == currentUser.Id);

                if (facultyProfile == null)
                    return NotFound("Faculty profile not found for current user.");

                return RedirectToAction(nameof(Details), new { id = facultyProfile.Id });
            }

            // Admin sees all
            var applicationDbContext = _context.FacultyProfiles.Include(f => f.Faculty);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: FacultyProfiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var facultyProfile = await _context.FacultyProfiles
                .Include(f => f.Faculty)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (facultyProfile == null)
                return NotFound();

            // If Faculty, hide IdentityUserId from view
            ViewData["ShowIdentityUser"] = User.IsInRole("Admin");

            return View(facultyProfile);
        }

        // GET: FacultyProfiles/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: FacultyProfiles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,IdentityUserId,Name,Email,Phone")] FacultyProfile facultyProfile)
        {
            if (ModelState.IsValid)
            {
                _context.Add(facultyProfile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", facultyProfile.IdentityUserId);
            return View(facultyProfile);
        }

        // GET: FacultyProfiles/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var facultyProfile = await _context.FacultyProfiles.FindAsync(id);
            if (facultyProfile == null)
                return NotFound();

            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", facultyProfile.IdentityUserId);
            return View(facultyProfile);
        }

        // POST: FacultyProfiles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdentityUserId,Name,Email,Phone")] FacultyProfile facultyProfile)
        {
            if (id != facultyProfile.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(facultyProfile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacultyProfileExists(facultyProfile.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", facultyProfile.IdentityUserId);
            return View(facultyProfile);
        }

        // GET: FacultyProfiles/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var facultyProfile = await _context.FacultyProfiles
                .Include(f => f.Faculty)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (facultyProfile == null)
                return NotFound();

            return View(facultyProfile);
        }

        // POST: FacultyProfiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var facultyProfile = await _context.FacultyProfiles.FindAsync(id);
            if (facultyProfile != null)
                _context.FacultyProfiles.Remove(facultyProfile);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FacultyProfileExists(int id)
        {
            return _context.FacultyProfiles.Any(e => e.Id == id);
        }
    }
}