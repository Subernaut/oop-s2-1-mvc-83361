using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Library.Domain;
using Library.MVC.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Library.MVC.Controllers
{
    [Authorize(Roles = "Student,Faculty,Admin")]
    public class StudentProfilesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public StudentProfilesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: StudentProfiles
        public async Task<IActionResult> Index()
        {
            var currentUserId = _userManager.GetUserId(User);

            if (User.IsInRole("Admin"))
            {
                // Admin sees all
                var adminProfiles = _context.StudentProfiles.Include(s => s.Student);
                return View(await adminProfiles.ToListAsync());
            }
            else if (User.IsInRole("Faculty"))
            {
                // Faculty sees only their students
                var facultyProfile = await _context.FacultyProfiles
                    .Include(f => f.Courses)
                    .FirstOrDefaultAsync(f => f.IdentityUserId == currentUserId);

                if (facultyProfile == null)
                    return Forbid();

                var studentProfiles = _context.StudentProfiles
                    .Where(s => s.CourseEnrolments.Any(ce => facultyProfile.Courses.Contains(ce.Course)))
                    .ToList();

                return View(studentProfiles);
            }
            else if (User.IsInRole("Student"))
            {
                // Student sees only their profile -> redirect to Details
                var studentProfile = await _context.StudentProfiles
                    .FirstOrDefaultAsync(s => s.IdentityUserId == currentUserId);

                if (studentProfile == null)
                    return NotFound();

                return RedirectToAction(nameof(Details), new { id = studentProfile.Id });
            }

            return Forbid();
        }

        // GET: StudentProfiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var currentUserId = _userManager.GetUserId(User);

            var studentProfile = await _context.StudentProfiles
                .Include(s => s.Student)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (studentProfile == null)
                return NotFound();

            if (User.IsInRole("Admin"))
            {
                // Admin can see everything
                return View(studentProfile);
            }
            else if (User.IsInRole("Faculty"))
            {
                var facultyProfile = await _context.FacultyProfiles
                    .Include(f => f.Courses)
                    .FirstOrDefaultAsync(f => f.IdentityUserId == currentUserId);

                if (facultyProfile == null)
                    return Forbid();

                bool isStudentOfFaculty = _context.CourseEnrolments
                    .Any(ce => ce.StudentProfileId == studentProfile.Id && facultyProfile.Courses.Contains(ce.Course));

                if (!isStudentOfFaculty)
                    return Forbid();

                return View(studentProfile); 
            }
            else if (User.IsInRole("Student"))
            {
                if (studentProfile.IdentityUserId != currentUserId)
                    return Forbid();

                return View(studentProfile);
            }

            return Forbid();
        }

        // GET: StudentProfiles/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            if (!User.IsInRole("Admin"))
                return Forbid();

            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: StudentProfiles/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdentityUserId,Name,Email,Phone,Address,StudentNumber")] StudentProfile studentProfile)
        {
            if (!User.IsInRole("Admin"))
                return Forbid();

            if (ModelState.IsValid)
            {
                _context.Add(studentProfile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", studentProfile.IdentityUserId);
            return View(studentProfile);
        }

        // GET: StudentProfiles/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!User.IsInRole("Admin"))
                return Forbid();

            if (id == null)
                return NotFound();

            var studentProfile = await _context.StudentProfiles.FindAsync(id);
            if (studentProfile == null)
                return NotFound();

            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", studentProfile.IdentityUserId);
            return View(studentProfile);
        }

        // POST: StudentProfiles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdentityUserId,Name,Email,Phone,Address,StudentNumber")] StudentProfile studentProfile)
        {
            if (!User.IsInRole("Admin"))
                return Forbid();

            if (id != studentProfile.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(studentProfile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentProfileExists(studentProfile.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", studentProfile.IdentityUserId);
            return View(studentProfile);
        }

        // GET: StudentProfiles/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!User.IsInRole("Admin"))
                return Forbid();

            if (id == null)
                return NotFound();

            var studentProfile = await _context.StudentProfiles
                .Include(s => s.Student)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (studentProfile == null)
                return NotFound();

            return View(studentProfile);
        }

        // POST: StudentProfiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!User.IsInRole("Admin"))
                return Forbid();

            var studentProfile = await _context.StudentProfiles.FindAsync(id);
            if (studentProfile != null)
            {
                _context.StudentProfiles.Remove(studentProfile);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentProfileExists(int id)
        {
            return _context.StudentProfiles.Any(e => e.Id == id);
        }
    }
}