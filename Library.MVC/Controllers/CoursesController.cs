using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Library.Domain;
using Library.MVC.Data;
using Microsoft.AspNetCore.Authorization;

namespace Library.MVC.Controllers
{
    [Authorize(Roles = "Admin,Faculty,Student")]
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CoursesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Courses
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            IQueryable<Course> coursesQuery = _context.Courses
                .Include(c => c.Branch)
                .Include(c => c.Faculty)
                .Include(c => c.Enrolments)
                    .ThenInclude(e => e.StudentProfile);

            if (User.IsInRole("Student"))
            {
                var studentProfile = await _context.StudentProfiles.FirstOrDefaultAsync(s => s.IdentityUserId == userId);
                if (studentProfile != null)
                {
                    coursesQuery = coursesQuery.Where(c =>
                        c.Enrolments.Any(e => e.StudentProfileId == studentProfile.Id));
                }
            }
            else if (User.IsInRole("Faculty"))
            {
                var facultyProfile = await _context.FacultyProfiles.FirstOrDefaultAsync(f => f.IdentityUserId == userId);
                if (facultyProfile != null)
                {
                    coursesQuery = coursesQuery.Where(c => c.FacultyId == facultyProfile.Id);
                }
            }

            var coursesList = await coursesQuery.ToListAsync();
            return View(coursesList);
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var course = await _context.Courses
                .Include(c => c.Branch)
                .Include(c => c.Faculty)
                .Include(c => c.Enrolments)
                    .ThenInclude(e => e.StudentProfile)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null) return NotFound();

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (User.IsInRole("Student") &&
                !course.Enrolments.Any(e => e.StudentProfile.IdentityUserId == userId))
            {
                return Forbid();
            }

            if (User.IsInRole("Faculty") &&
                course.Faculty.IdentityUserId != userId)
            {
                return Forbid();
            }

            return View(course);
        }

        // ADMIN ONLY: Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["BranchId"] = new SelectList(_context.Branches, "Id", "Name");
            ViewData["FacultyId"] = new SelectList(_context.FacultyProfiles, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,BranchId,StartDate,EndDate,FacultyId")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BranchId"] = new SelectList(_context.Branches, "Id", "Name", course.BranchId);
            ViewData["FacultyId"] = new SelectList(_context.FacultyProfiles, "Id", "Name", course.FacultyId);
            return View(course);
        }

        // ADMIN ONLY: Edit
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            ViewData["BranchId"] = new SelectList(_context.Branches, "Id", "Name", course.BranchId);
            ViewData["FacultyId"] = new SelectList(_context.FacultyProfiles, "Id", "Name", course.FacultyId);
            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,BranchId,StartDate,EndDate,FacultyId")] Course course)
        {
            if (id != course.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["BranchId"] = new SelectList(_context.Branches, "Id", "Name", course.BranchId);
            ViewData["FacultyId"] = new SelectList(_context.FacultyProfiles, "Id", "Name", course.FacultyId);
            return View(course);
        }

        // ADMIN ONLY: Delete
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var course = await _context.Courses
                .Include(c => c.Branch)
                .Include(c => c.Faculty)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (course == null) return NotFound();

            return View(course);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }
    }
}