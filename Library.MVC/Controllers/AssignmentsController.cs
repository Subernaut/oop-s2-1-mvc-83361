using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Domain;
using Library.MVC.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Library.MVC.Controllers
{
    [Authorize(Roles = "Student,Faculty,Admin")]
    public class AssignmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser>
    _userManager;

    public AssignmentsController(ApplicationDbContext context, UserManager<IdentityUser>
        userManager)
        {
        _context = context;
        _userManager = userManager;
        }

        // GET: Assignments
        public async Task<IActionResult>
            Index()
            {
            var userId = _userManager.GetUserId(User);
            var roles = await _userManager.GetRolesAsync(await _userManager.GetUserAsync(User));
            var role = roles.FirstOrDefault() ?? "Unknown";

            IQueryable<Assignment>
                query = _context.Assignments
                .Include(a => a.Course)
                .Include(a => a.AssignmentResults)
                .ThenInclude(r => r.StudentProfile);

                if (role == "Student")
                {
                query = query.Where(a => a.AssignmentResults
                .Any(r => r.StudentProfile.IdentityUserId == userId));
                }
                else if (role == "Faculty")
                {
                query = query.Where(a => a.Course.Faculty.IdentityUserId == userId);
                }

                var assignments = await query.ToListAsync();

                ViewData["Role"] = role;

                return View(assignments);
                }

        // GET: Assignments/Details/5
        [Authorize(Roles = "Faculty,Admin")]
        public async Task<IActionResult>
                    Details(int? id)
                    {
                    if (id == null) return NotFound();

                    var assignment = await _context.Assignments
                    .Include(a => a.Course)
                    .Include(a => a.AssignmentResults)
                    .ThenInclude(r => r.StudentProfile)
                    .FirstOrDefaultAsync(a => a.Id == id);

                    if (assignment == null) return NotFound();

                    return View(assignment);
                    }

        // GET: Assignments/Create
        [Authorize(Roles = "Faculty,Admin")]
        public IActionResult Create()
                    {
                    ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name");
                    return View();
                    }

                    // POST: Assignments/Create
                    [HttpPost]
                    [ValidateAntiForgeryToken]
        [Authorize(Roles = "Faculty,Admin")]
        public async Task<IActionResult>
                        Create([Bind("Id,CourseId,Title,MaxScore,DueDate")] Assignment assignment)
                        {
                        if (ModelState.IsValid)
                        {
                        _context.Add(assignment);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                        }
                        ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name", assignment.CourseId);
                        return View(assignment);
                        }

        // GET: Assignments/Edit/5
        [Authorize(Roles = "Faculty,Admin")]
        public async Task<IActionResult>
                            Edit(int? id)
                            {
                            if (id == null) return NotFound();

                            var assignment = await _context.Assignments.FindAsync(id);
                            if (assignment == null) return NotFound();

                            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name", assignment.CourseId);
                            return View(assignment);
                            }

        // POST: Assignments/Edit/5
        [Authorize(Roles = "Faculty,Admin")]
        [HttpPost]
                            [ValidateAntiForgeryToken]
                            public async Task<IActionResult>
                                Edit(int id, [Bind("Id,CourseId,Title,MaxScore,DueDate")] Assignment assignment)
                                {
                                if (id != assignment.Id) return NotFound();

                                if (ModelState.IsValid)
                                {
                                try
                                {
                                _context.Update(assignment);
                                await _context.SaveChangesAsync();
                                }
                                catch (DbUpdateConcurrencyException)
                                {
                                if (!AssignmentExists(assignment.Id)) return NotFound();
                                else throw;
                                }
                                return RedirectToAction(nameof(Index));
                                }
                                ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name", assignment.CourseId);
                                return View(assignment);
                                }

        // GET: Assignments/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult>
                                    Delete(int? id)
                                    {
                                    if (id == null) return NotFound();

                                    var assignment = await _context.Assignments
                                    .Include(a => a.Course)
                                    .FirstOrDefaultAsync(m => m.Id == id);

                                    if (assignment == null) return NotFound();

                                    return View(assignment);
                                    }

        // POST: Assignments/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
                                    [ValidateAntiForgeryToken]
                                    public async Task<IActionResult>
                                        DeleteConfirmed(int id)
                                        {
                                        var assignment = await _context.Assignments.FindAsync(id);
                                        if (assignment != null)
                                        {
                                        // Delete linked AssignmentResults first
                                        var results = _context.AssignmentResults.Where(r => r.AssignmentId == id);
                                        _context.AssignmentResults.RemoveRange(results);

                                        _context.Assignments.Remove(assignment);
                                        }

                                        await _context.SaveChangesAsync();
                                        return RedirectToAction(nameof(Index));
                                        }

                                        private bool AssignmentExists(int id)
                                        {
                                        return _context.Assignments.Any(e => e.Id == id);
                                        }
                                        }
                                        }
