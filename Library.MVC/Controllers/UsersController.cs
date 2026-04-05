using Library.Domain;
using Library.MVC.Data;
using Library.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Library.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // ===================== INDEX =====================
        public async Task<IActionResult> Index()
        {
            var usersList = new List<UserViewModel>();
            var allUsers = await _userManager.Users.ToListAsync();

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault() ?? "Unknown";

                var viewModel = new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Role = role,
                    HasProfile = false
                };

                if (role == "Student")
                {
                    var student = await _context.StudentProfiles
                        .FirstOrDefaultAsync(s => s.IdentityUserId == user.Id);
                    if (student != null) viewModel.HasProfile = true;
                }
                else if (role == "Faculty")
                {
                    var faculty = await _context.FacultyProfiles
                        .FirstOrDefaultAsync(f => f.IdentityUserId == user.Id);
                    if (faculty != null) viewModel.HasProfile = true;
                }

                usersList.Add(viewModel);
            }

            return View(usersList);
        }

        // ===================== DETAILS =====================
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            object? profile = null;

            if (roles.Contains("Student"))
                profile = await _context.StudentProfiles.FirstOrDefaultAsync(s => s.IdentityUserId == id);
            else if (roles.Contains("Faculty"))
                profile = await _context.FacultyProfiles.FirstOrDefaultAsync(f => f.IdentityUserId == id);

            return View(new { User = user, Roles = roles, Profile = profile });
        }

        // ===================== CREATE =====================
        public IActionResult Create()
        {
            ViewData["Roles"] = new SelectList(new[] { "Admin", "Student", "Faculty" });
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string email, string password, string role)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(role))
            {
                ModelState.AddModelError("", "Email, Password, and Role are required.");
                ViewData["Roles"] = new SelectList(new[] { "Admin", "Student", "Faculty" });
                return View();
            }

            var user = new IdentityUser { UserName = email, Email = email };
            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                ViewData["Roles"] = new SelectList(new[] { "Admin", "Student", "Faculty" });
                return View();
            }

            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            await _userManager.AddToRoleAsync(user, role);

            return RedirectToAction(nameof(Index));
        }

        // ===================== EDIT =====================
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            ViewData["Roles"] = new[] { "Admin", "Student", "Faculty" };

            return View(new { User = user, Roles = roles });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string email, string role)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(role))
            {
                ModelState.AddModelError("", "All fields are required.");
                ViewData["Roles"] = new[] { "Admin", "Student", "Faculty" };
                return View();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            var oldRole = currentRoles.FirstOrDefault() ?? "";

            // Delete profile if role changes away from Student/Faculty
            if (oldRole == "Student" && role != "Student")
            {
                var studentProfile = await _context.StudentProfiles
                    .FirstOrDefaultAsync(s => s.IdentityUserId == id);
                if (studentProfile != null)
                    _context.StudentProfiles.Remove(studentProfile);
            }
            else if (oldRole == "Faculty" && role != "Faculty")
            {
                var facultyProfile = await _context.FacultyProfiles
                    .FirstOrDefaultAsync(f => f.IdentityUserId == id);
                if (facultyProfile != null)
                    _context.FacultyProfiles.Remove(facultyProfile);
            }

            // Update email
            user.Email = email;
            user.UserName = email;
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                    ModelState.AddModelError("", error.Description);

                ViewData["Roles"] = new[] { "Admin", "Student", "Faculty" };
                return View(new { User = user, Roles = currentRoles });
            }

            // Ensure role exists
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            // Replace roles
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, role);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ===================== DELETE =====================
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            object? profile = null;

            if (roles.Contains("Student"))
                profile = await _context.StudentProfiles.FirstOrDefaultAsync(s => s.IdentityUserId == id);
            else if (roles.Contains("Faculty"))
                profile = await _context.FacultyProfiles.FirstOrDefaultAsync(f => f.IdentityUserId == id);

            return View(new { User = user, Roles = roles, Profile = profile });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            // Delete linked profile
            if (roles.Contains("Student"))
            {
                var studentProfile = await _context.StudentProfiles
                    .FirstOrDefaultAsync(s => s.IdentityUserId == id);
                if (studentProfile != null)
                    _context.StudentProfiles.Remove(studentProfile);
            }
            else if (roles.Contains("Faculty"))
            {
                var facultyProfile = await _context.FacultyProfiles
                    .FirstOrDefaultAsync(f => f.IdentityUserId == id);
                if (facultyProfile != null)
                    _context.FacultyProfiles.Remove(facultyProfile);
            }

            // Delete Identity user
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return RedirectToAction(nameof(Index));
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}