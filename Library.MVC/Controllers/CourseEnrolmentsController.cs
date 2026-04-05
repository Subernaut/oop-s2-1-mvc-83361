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
    [Authorize(Roles = "Student,Faculty,Admin")]
    public class CourseEnrolmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CourseEnrolmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CourseEnrolments
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            IQueryable<CourseEnrolment> enrolmentsQuery = _context.CourseEnrolments
                .Include(e => e.Course)
                    .ThenInclude(c => c.Faculty)
                .Include(e => e.StudentProfile);

            if (User.IsInRole("Student"))
            {
                // Only show this student's enrolments
                var student = await _context.StudentProfiles.FirstOrDefaultAsync(s => s.IdentityUserId == userId);
                if (student != null)
                {
                    enrolmentsQuery = enrolmentsQuery.Where(e => e.StudentProfileId == student.Id);
                }
            }
            else if (User.IsInRole("Faculty"))
            {
                // Only show enrolments for courses this faculty teaches
                var faculty = await _context.FacultyProfiles.FirstOrDefaultAsync(f => f.IdentityUserId == userId);
                if (faculty != null)
                {
                    enrolmentsQuery = enrolmentsQuery.Where(e => e.Course.FacultyId == faculty.Id);
                }
            }
            // Admin sees all, no filter

            var enrolments = await enrolmentsQuery
                .OrderBy(e => e.Course.Name)
                .ThenBy(e => e.StudentProfile.Name)
                .ToListAsync();

            return View(enrolments);
        }

        // GET: CourseEnrolments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var enrolment = await _context.CourseEnrolments
                .Include(e => e.Course)
                    .ThenInclude(c => c.Faculty)
                .Include(e => e.StudentProfile)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enrolment == null) return NotFound();

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            // Restrict access
            if (User.IsInRole("Student") && enrolment.StudentProfile.IdentityUserId != userId)
                return Forbid();
            if (User.IsInRole("Faculty") && enrolment.Course.Faculty.IdentityUserId != userId)
                return Forbid();

            return View(enrolment);
        }

        // GET: CourseEnrolments/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name");
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Name");
            return View();
        }

        // POST: CourseEnrolments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,StudentProfileId,CourseId,EnrolDate,Status")] CourseEnrolment courseEnrolment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(courseEnrolment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name", courseEnrolment.CourseId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Name", courseEnrolment.StudentProfileId);
            return View(courseEnrolment);
        }

        // GET: CourseEnrolments/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var enrolment = await _context.CourseEnrolments.FindAsync(id);
            if (enrolment == null) return NotFound();

            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name", enrolment.CourseId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Name", enrolment.StudentProfileId);
            return View(enrolment);
        }

        // POST: CourseEnrolments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentProfileId,CourseId,EnrolDate,Status")] CourseEnrolment courseEnrolment)
        {
            if (id != courseEnrolment.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(courseEnrolment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseEnrolmentExists(courseEnrolment.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name", courseEnrolment.CourseId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Name", courseEnrolment.StudentProfileId);
            return View(courseEnrolment);
        }

        // GET: CourseEnrolments/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var enrolment = await _context.CourseEnrolments
                .Include(e => e.Course)
                .Include(e => e.StudentProfile)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (enrolment == null) return NotFound();
            return View(enrolment);
        }

        // POST: CourseEnrolments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enrolment = await _context.CourseEnrolments.FindAsync(id);
            if (enrolment != null)
            {
                _context.CourseEnrolments.Remove(enrolment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CourseEnrolmentExists(int id)
        {
            return _context.CourseEnrolments.Any(e => e.Id == id);
        }
    }
}