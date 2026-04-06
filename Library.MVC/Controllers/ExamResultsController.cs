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
        // HELPER:
        private async Task<List<SelectListItem>> GetAvailableExams(int? studentId, int? currentExamId = null)
        {
            var exams = await _context.Exams.ToListAsync();

            if (studentId == null)
            {
                return exams.Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.Title
                }).ToList();
            }

            var takenExamIds = await _context.ExamResults
                .Where(r => r.StudentProfileId == studentId)
                .Select(r => r.ExamId)
                .ToListAsync();

            return exams
                .Where(e => !takenExamIds.Contains(e.Id) || e.Id == currentExamId)
                .Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.Title
                })
                .ToList();
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
                {
                    query = query.Where(r =>
                        r.StudentProfileId == studentProfile.Id &&
                        r.Exam != null &&
                        r.Exam.ResultsReleased
                    );
                }
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

            if (User.IsInRole("Student"))
            {
                var studentProfile = await _context.StudentProfiles
                    .FirstOrDefaultAsync(s => s.IdentityUserId == User.GetUserId());

                if (studentProfile == null || examResult.StudentProfileId != studentProfile.Id)
                    return Forbid();

                if (examResult.Exam == null || !examResult.Exam.ResultsReleased)
                    return Forbid();
            }
            else if (User.IsInRole("Faculty"))
            {
                var facultyProfile = await _context.FacultyProfiles
                    .Include(f => f.Courses)
                    .FirstOrDefaultAsync(f => f.IdentityUserId == User.GetUserId());

                if (facultyProfile == null ||
                    !facultyProfile.Courses.Select(c => c.Id).Contains(examResult.Exam.CourseId))
                    return Forbid();
            }

            return View(examResult);
        }

        // GET: ExamResults/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Name");

            // no student selected yet → show all exams
            ViewData["ExamId"] = new SelectList(await GetAvailableExams(null), "Value", "Text");

            return View();
        }

        // POST: ExamResults/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,StudentProfileId,ExamId,Score,EndDate")] ExamResult examResult)
        {
            if (examResult.StudentProfileId == null || examResult.ExamId == 0)
            {
                ModelState.AddModelError("", "Student and Exam must be selected.");
            }

            var exam = await _context.Exams
                .FirstOrDefaultAsync(e => e.Id == examResult.ExamId);

            if (exam == null)
            {
                ModelState.AddModelError("", "Selected exam does not exist.");
            }
            else
            {
                if (examResult.Score < 0 || examResult.Score > exam.MaxScore)
                {
                    ModelState.AddModelError("Score", $"Score must be between 0 and {exam.MaxScore}.");
                }
            }

            // ✅ duplicate check (safe)
            if (examResult.StudentProfileId != null && examResult.ExamId != 0)
            {
                bool exists = await _context.ExamResults
                    .AnyAsync(r => r.StudentProfileId == examResult.StudentProfileId &&
                                   r.ExamId == examResult.ExamId);

                if (exists)
                {
                    ModelState.AddModelError("", "This student already has a result for this exam.");
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(examResult);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // 🔥 ALWAYS repopulate safely
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Name", examResult.StudentProfileId);

            ViewData["ExamId"] = new SelectList(
                await GetAvailableExams(examResult.StudentProfileId),
                "Value",
                "Text",
                examResult.ExamId
            );

            return View(examResult);
        }

        // GET: ExamResults/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var examResult = await _context.ExamResults.FindAsync(id);
            if (examResult == null) return NotFound();

            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Name", examResult.StudentProfileId);

            ViewData["ExamId"] = new SelectList(
                await GetAvailableExams(examResult.StudentProfileId, examResult.ExamId),
                "Value",
                "Text",
                examResult.ExamId
            );

            return View(examResult);
        }

        // POST: ExamResults/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentProfileId,ExamId,Score,EndDate")] ExamResult examResult)
        {
            if (id != examResult.Id) return NotFound();
            var exam = await _context.Exams.FirstOrDefaultAsync(e => e.Id == examResult.ExamId);

            if (exam == null)
            {
                ModelState.AddModelError("", "Invalid exam.");
            }
            else if (examResult.Score < 0 || examResult.Score > exam.MaxScore)
            {
                ModelState.AddModelError("Score", $"Score must be between 0 and {exam.MaxScore}.");
            }

            // ✅ prevent duplicate (ignore itself)
            bool exists = await _context.ExamResults
                .AnyAsync(r =>
                    r.StudentProfileId == examResult.StudentProfileId &&
                    r.ExamId == examResult.ExamId &&
                    r.Id != examResult.Id);

            if (exists)
            {
                ModelState.AddModelError("", "This student already has a result for this exam.");
            }

            if (ModelState.IsValid)
            {
                _context.Update(examResult);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Name", examResult.StudentProfileId);

            ViewData["ExamId"] = new SelectList(
                await GetAvailableExams(examResult.StudentProfileId, examResult.ExamId),
                "Value",
                "Text",
                examResult.ExamId
            );

            return View(examResult);
        }

        // GET: ExamResults/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
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