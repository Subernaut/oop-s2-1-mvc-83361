using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Library.Domain;
using Library.MVC.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Library.MVC.Controllers
{
    [Authorize(Roles = "Student,Faculty,Admin")]
    public class ExamsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExamsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Exams
        public async Task<IActionResult> Index()
        {
            IQueryable<Exam> examsQuery = _context.Exams
                .Include(e => e.Course)
                .ThenInclude(c => c.Enrolments)
                .ThenInclude(ce => ce.StudentProfile);

            if (User.IsInRole("Student"))
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                examsQuery = examsQuery.Where(e => e.Course.Enrolments
                    .Any(ce => ce.StudentProfile.IdentityUserId == userId));
            }
            else if (User.IsInRole("Faculty"))
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                examsQuery = examsQuery.Where(e => e.Course.Faculty.IdentityUserId == userId);
            }
            // Admin sees all

            return View(await examsQuery.ToListAsync());
        }

        // GET: Exams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var exam = await _context.Exams
                .Include(e => e.Course)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (exam == null)
                return NotFound();

            if (User.IsInRole("Admin") || User.IsInRole("Faculty"))
            {
                // Admins and Faculty see all results for the exam
                var results = await _context.ExamResults
                    .Include(r => r.StudentProfile)
                    .Where(r => r.ExamId == exam.Id)
                    .ToListAsync();

                ViewData["ExamResults"] = results;
            }
            else if (User.IsInRole("Student"))
            {
                // Students see only their own exam result
                var studentProfile = await _context.StudentProfiles
                    .FirstOrDefaultAsync(s => s.IdentityUserId == User.GetUserId());

                if (studentProfile != null)
                {
                    var myResult = await _context.ExamResults
                        .FirstOrDefaultAsync(r => r.ExamId == exam.Id && r.StudentProfileId == studentProfile.Id);

                    ViewData["MyExamResult"] = myResult;
                }
            }

            return View(exam);
        }



        // GET: Exams/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name");
            return View();
        }

        // POST: Exams/Create
        [HttpPost, Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,CourseId,Date,MaxScore,ResultsReleased")] Exam exam)
        {
            if (ModelState.IsValid)
            {
                _context.Add(exam);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name", exam.CourseId);
            return View(exam);
        }

        // GET: Exams/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var exam = await _context.Exams.FindAsync(id);
            if (exam == null)
                return NotFound();

            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name", exam.CourseId);
            return View(exam);
        }

        // POST: Exams/Edit/5
        [HttpPost, Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,CourseId,Date,MaxScore,ResultsReleased")] Exam exam)
        {
            if (id != exam.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(exam);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExamExists(exam.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name", exam.CourseId);
            return View(exam);
        }

        // GET: Exams/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var exam = await _context.Exams
                .Include(e => e.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (exam == null)
                return NotFound();

            return View(exam);
        }

        // POST: Exams/Delete/5
        [HttpPost, ActionName("Delete"), Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exam = await _context.Exams.FindAsync(id);
            if (exam != null)
            {
                _context.Exams.Remove(exam);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ExamExists(int id)
        {
            return _context.Exams.Any(e => e.Id == id);
        }
    }

    // Helper extension to get current user Id
    public static class UserExtensions
    {
        public static string GetUserId(this System.Security.Claims.ClaimsPrincipal user)
        {
            return user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";
        }
    }
}