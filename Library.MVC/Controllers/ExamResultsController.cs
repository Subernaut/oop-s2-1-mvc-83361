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

namespace Library.MVC.Controllers
{
    [Authorize(Roles = "Student,Faculty,Admin")]
    public class ExamResultsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExamResultsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ExamResults
        public async Task<IActionResult> Index()
        {
            IQueryable<ExamResult> query = _context.ExamResults
                .Include(e => e.Exam)
                .ThenInclude(ex => ex.Course)
                .Include(e => e.StudentProfile);

            if (User.IsInRole("Student"))
            {
                var studentProfile = await _context.StudentProfiles
                    .FirstOrDefaultAsync(s => s.IdentityUserId == User.GetUserId());

                if (studentProfile != null)
                    query = query.Where(r => r.StudentProfileId == studentProfile.Id);
            }
            else if (User.IsInRole("Faculty"))
            {
                var facultyProfile = await _context.FacultyProfiles
                    .Include(f => f.Courses)
                    .FirstOrDefaultAsync(f => f.IdentityUserId == User.GetUserId());

                if (facultyProfile != null)
                {
                    var facultyCourseIds = facultyProfile.Courses.Select(c => c.Id).ToList();
                    query = query.Where(r => facultyCourseIds.Contains(r.Exam.CourseId));
                }
            }
            // Admin sees everything

            var examResults = await query.ToListAsync();
            return View(examResults);
        }

        // GET: ExamResults/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var examResult = await _context.ExamResults
                .Include(e => e.Exam)
                .ThenInclude(ex => ex.Course)
                .Include(e => e.StudentProfile)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (examResult == null) return NotFound();

            // Role-based access
            if (User.IsInRole("Student"))
            {
                var studentProfile = await _context.StudentProfiles
                    .FirstOrDefaultAsync(s => s.IdentityUserId == User.GetUserId());

                if (studentProfile == null || examResult.StudentProfileId != studentProfile.Id)
                    return Forbid(); // cannot view others' results
            }
            else if (User.IsInRole("Faculty"))
            {
                var facultyProfile = await _context.FacultyProfiles
                    .Include(f => f.Courses)
                    .FirstOrDefaultAsync(f => f.IdentityUserId == User.GetUserId());

                if (facultyProfile == null ||
                    !facultyProfile.Courses.Select(c => c.Id).Contains(examResult.Exam.CourseId))
                    return Forbid(); // cannot view results outside their courses
            }

            return View(examResult);
        }

        // GET: ExamResults/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            if (!User.IsInRole("Admin"))
                return Forbid(); // Only admins can create

            ViewData["ExamId"] = new SelectList(_context.Exams, "Id", "Title");
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "FullName");
            return View();
        }

        // POST: ExamResults/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,StudentProfileId,ExamId,Score,Grade,EndDate")] ExamResult examResult)
        {
            if (!User.IsInRole("Admin")) return Forbid();

            if (ModelState.IsValid)
            {
                _context.Add(examResult);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ExamId"] = new SelectList(_context.Exams, "Id", "Title", examResult.ExamId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "FullName", examResult.StudentProfileId);
            return View(examResult);
        }

        // GET: ExamResults/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!User.IsInRole("Admin")) return Forbid();
            if (id == null) return NotFound();

            var examResult = await _context.ExamResults.FindAsync(id);
            if (examResult == null) return NotFound();

            ViewData["ExamId"] = new SelectList(_context.Exams, "Id", "Title", examResult.ExamId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "FullName", examResult.StudentProfileId);
            return View(examResult);
        }

        // POST: ExamResults/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentProfileId,ExamId,Score,Grade,EndDate")] ExamResult examResult)
        {
            if (!User.IsInRole("Admin")) return Forbid();
            if (id != examResult.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(examResult);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExamResultExists(examResult.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ExamId"] = new SelectList(_context.Exams, "Id", "Title", examResult.ExamId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "FullName", examResult.StudentProfileId);
            return View(examResult);
        }

        // GET: ExamResults/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!User.IsInRole("Admin")) return Forbid();
            if (id == null) return NotFound();

            var examResult = await _context.ExamResults
                .Include(e => e.Exam)
                .Include(e => e.StudentProfile)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (examResult == null) return NotFound();

            return View(examResult);
        }

        // POST: ExamResults/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!User.IsInRole("Admin")) return Forbid();

            var examResult = await _context.ExamResults.FindAsync(id);
            if (examResult != null)
                _context.ExamResults.Remove(examResult);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExamResultExists(int id)
        {
            return _context.ExamResults.Any(e => e.Id == id);
        }
    }
}