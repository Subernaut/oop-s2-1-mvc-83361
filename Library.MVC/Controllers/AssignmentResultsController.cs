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
using System.Security.Claims;

namespace Library.MVC.Controllers
{
    [Authorize(Roles = "Student,Faculty,Admin")]
    public class AssignmentResultsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssignmentResultsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AssignmentResults
        [Authorize(Roles = "Faculty,Admin,Student")]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.IsInRole("Admin") ? "Admin" :
                       User.IsInRole("Faculty") ? "Faculty" : "Student";

            IQueryable<AssignmentResult> resultsQuery = _context.AssignmentResults
                .Include(ar => ar.Assignment)
                    .ThenInclude(a => a.Course)
                        .ThenInclude(c => c.Faculty)
                .Include(ar => ar.StudentProfile)
                .AsQueryable();

            // Faculty: only results for assignments in their courses
            if (role == "Faculty")
            {
                resultsQuery = resultsQuery
                    .Where(ar => ar.Assignment.Course.Faculty.IdentityUserId == userId);
            }

            // Student: only their own results
            if (role == "Student")
            {
                resultsQuery = resultsQuery
                    .Where(ar => ar.StudentProfile.IdentityUserId == userId);
            }

            var results = await resultsQuery
                .OrderByDescending(ar => ar.Assignment.DueDate)
                .ToListAsync();

            ViewData["Role"] = role;
            ViewData["UserId"] = userId;

            return View(results);
        }

        // GET: AssignmentResults/Details/5
        [Authorize(Roles = "Student,Faculty,Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignmentResult = await _context.AssignmentResults
                .Include(ar => ar.Assignment)
                    .ThenInclude(a => a.Course)
                        .ThenInclude(c => c.Faculty)
                .Include(ar => ar.StudentProfile)
                .FirstOrDefaultAsync(ar => ar.Id == id);

            if (assignmentResult == null)
            {
                return NotFound();
            }

            // Restrict student access to only their own result
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (User.IsInRole("Student") && assignmentResult.StudentProfile.IdentityUserId != userId)
            {
                return Forbid();
            }

            ViewData["Role"] = User.IsInRole("Admin") ? "Admin" :
                               User.IsInRole("Faculty") ? "Faculty" : "Student";

            return View(assignmentResult);
        }

        // GET: AssignmentResults/Create
        [Authorize(Roles = "Faculty,Admin")]
        public IActionResult Create()
        {
            ViewData["AssignmentId"] = new SelectList(_context.Assignments, "Id", "Title");
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Name");
            return View();
        }

        // POST: AssignmentResults/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Faculty,Admin")]
        public async Task<IActionResult> Create([Bind("Id,AssignmentId,StudentProfileId,Score,Feedback")] AssignmentResult assignmentResult)
        {
            if (ModelState.IsValid)
            {
                _context.Add(assignmentResult);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AssignmentId"] = new SelectList(_context.Assignments, "Id", "Title", assignmentResult.AssignmentId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Name", assignmentResult.StudentProfileId);
            return View(assignmentResult);
        }

        // GET: AssignmentResults/Edit/5
        [Authorize(Roles = "Faculty,Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var assignmentResult = await _context.AssignmentResults.FindAsync(id);
            if (assignmentResult == null) return NotFound();

            ViewData["AssignmentId"] = new SelectList(_context.Assignments, "Id", "Title", assignmentResult.AssignmentId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Name", assignmentResult.StudentProfileId);
            return View(assignmentResult);
        }

        // POST: AssignmentResults/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Faculty,Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AssignmentId,StudentProfileId,Score,Feedback")] AssignmentResult assignmentResult)
        {
            if (id != assignmentResult.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(assignmentResult);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssignmentResultExists(assignmentResult.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AssignmentId"] = new SelectList(_context.Assignments, "Id", "Title", assignmentResult.AssignmentId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Name", assignmentResult.StudentProfileId);
            return View(assignmentResult);
        }

        // GET: AssignmentResults/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var assignmentResult = await _context.AssignmentResults
                .Include(ar => ar.Assignment)
                .Include(ar => ar.StudentProfile)
                .FirstOrDefaultAsync(ar => ar.Id == id);

            if (assignmentResult == null) return NotFound();

            return View(assignmentResult);
        }

        // POST: AssignmentResults/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assignmentResult = await _context.AssignmentResults.FindAsync(id);
            if (assignmentResult != null)
                _context.AssignmentResults.Remove(assignmentResult);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AssignmentResultExists(int id)
        {
            return _context.AssignmentResults.Any(e => e.Id == id);
        }
    }
}