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
    public class AttendanceRecordsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttendanceRecordsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AttendanceRecords
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var attendanceQuery = _context.AttendanceRecords
                .Include(a => a.CourseEnrolment)
                    .ThenInclude(e => e.Course)
                .Include(a => a.CourseEnrolment)
                    .ThenInclude(e => e.StudentProfile)
                .AsQueryable();

            if (User.IsInRole("Student"))
            {
                var studentProfile = await _context.StudentProfiles
                    .FirstOrDefaultAsync(s => s.IdentityUserId == userId);
                if (studentProfile != null)
                {
                    attendanceQuery = attendanceQuery.Where(a => a.CourseEnrolment.StudentProfileId == studentProfile.Id);
                }
            }
            else if (User.IsInRole("Faculty"))
            {
                var facultyProfile = await _context.FacultyProfiles
                    .FirstOrDefaultAsync(f => f.IdentityUserId == userId);
                if (facultyProfile != null)
                {
                    attendanceQuery = attendanceQuery.Where(a => a.CourseEnrolment.Course.FacultyId == facultyProfile.Id);
                }
            }
            // Admin sees all

            var attendanceList = await attendanceQuery.OrderByDescending(a => a.Date).ToListAsync();
            return View(attendanceList);
        }

        // GET: AttendanceRecords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var attendanceRecord = await _context.AttendanceRecords
                .Include(a => a.CourseEnrolment)
                    .ThenInclude(e => e.Course)
                .Include(a => a.CourseEnrolment)
                    .ThenInclude(e => e.StudentProfile)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (attendanceRecord == null) return NotFound();

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (User.IsInRole("Student") && attendanceRecord.CourseEnrolment.StudentProfile.IdentityUserId != userId)
                return Forbid();

            if (User.IsInRole("Faculty") && attendanceRecord.CourseEnrolment.Course.FacultyId !=
                (await _context.FacultyProfiles.FirstAsync(f => f.IdentityUserId == userId)).Id)
                return Forbid();

            return View(attendanceRecord);
        }

        // GET: AttendanceRecords/Create
        [Authorize(Roles = "Faculty,Admin")]
        public async Task<IActionResult> Create()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            IQueryable<CourseEnrolment> enrolmentsQuery = _context.CourseEnrolments
                .Include(e => e.StudentProfile)
                .Include(e => e.Course);

            if (User.IsInRole("Faculty"))
            {
                var facultyProfile = await _context.FacultyProfiles.FirstOrDefaultAsync(f => f.IdentityUserId == userId);
                enrolmentsQuery = enrolmentsQuery.Where(e => e.Course.FacultyId == facultyProfile.Id);
            }

            var enrolments = await enrolmentsQuery
                .Select(e => new { e.Id, Name = e.StudentProfile.Name + " - " + e.Course.Name })
                .ToListAsync();

            ViewData["CourseEnrolmentId"] = new SelectList(enrolments, "Id", "Name");
            return View();
        }

        // POST: AttendanceRecords/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Faculty,Admin")]
        public async Task<IActionResult> Create([Bind("Id,CourseEnrolmentId,Date,Present")] AttendanceRecord attendanceRecord)
        {
            if (ModelState.IsValid)
            {
                _context.Add(attendanceRecord);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CourseEnrolmentId"] = new SelectList(
                _context.CourseEnrolments.Select(e => new { e.Id, Name = e.StudentProfile.Name + " - " + e.Course.Name }),
                "Id", "Name",
                attendanceRecord.CourseEnrolmentId
            );

            return View(attendanceRecord);
        }

        // GET: AttendanceRecords/Edit/5
        [Authorize(Roles = "Faculty,Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var attendanceRecord = await _context.AttendanceRecords
                .Include(a => a.CourseEnrolment)
                    .ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (attendanceRecord == null) return NotFound();

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (User.IsInRole("Faculty") && attendanceRecord.CourseEnrolment.Course.FacultyId !=
                (await _context.FacultyProfiles.FirstAsync(f => f.IdentityUserId == userId)).Id)
                return Forbid();

            var enrolmentsQuery = _context.CourseEnrolments
                .Include(e => e.StudentProfile)
                .Include(e => e.Course);

            if (User.IsInRole("Faculty"))
            {
                var facultyProfile = await _context.FacultyProfiles.FirstAsync(f => f.IdentityUserId == userId);
                enrolmentsQuery = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<CourseEnrolment, Course?>)enrolmentsQuery.Where(e => e.Course.FacultyId == facultyProfile.Id);
            }

            var enrolments = await enrolmentsQuery
                .Select(e => new { e.Id, Name = e.StudentProfile.Name + " - " + e.Course.Name })
                .ToListAsync();

            ViewData["CourseEnrolmentId"] = new SelectList(enrolments, "Id", "Name", attendanceRecord.CourseEnrolmentId);
            return View(attendanceRecord);
        }

        // POST: AttendanceRecords/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Faculty,Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CourseEnrolmentId,Date,Present")] AttendanceRecord attendanceRecord)
        {
            if (id != attendanceRecord.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(attendanceRecord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AttendanceRecordExists(attendanceRecord.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["CourseEnrolmentId"] = new SelectList(_context.CourseEnrolments, "Id", "Id", attendanceRecord.CourseEnrolmentId);
            return View(attendanceRecord);
        }

        // GET: AttendanceRecords/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var attendanceRecord = await _context.AttendanceRecords
                .Include(a => a.CourseEnrolment)
                    .ThenInclude(e => e.Course)
                .Include(a => a.CourseEnrolment)
                    .ThenInclude(e => e.StudentProfile)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (attendanceRecord == null) return NotFound();

            return View(attendanceRecord);
        }

        // POST: AttendanceRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var attendanceRecord = await _context.AttendanceRecords.FindAsync(id);
            if (attendanceRecord != null)
            {
                _context.AttendanceRecords.Remove(attendanceRecord);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool AttendanceRecordExists(int id)
        {
            return _context.AttendanceRecords.Any(e => e.Id == id);
        }
    }
}