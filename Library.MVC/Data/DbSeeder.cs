using Bogus;
using Library.Domain;
using Library.MVC.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog.Parsing;

namespace Library.MVC.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        var rnd = new Random();
        var faker = new Faker();

        // -------------------------
        // 1️⃣ Roles
        string[] roles = { "Admin", "Faculty", "Student" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // -------------------------
        // 2️⃣ Admin
        const string adminEmail = "admin@college.com";
        const string adminPassword = "Admin123!";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
            await userManager.CreateAsync(adminUser, adminPassword);
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        // -------------------------
        // 3️⃣ Branches
        var branchData = new[]
        {
            new { Name = "Dublin Branch", Address = "1 College Street, Dublin" },
            new { Name = "Cork Branch", Address = "25 Main St, Cork" },
            new { Name = "Galway Branch", Address = "10 Harbour Rd, Galway" }
        };

        if (!await context.Branches.AnyAsync())
        {
            foreach (var b in branchData)
            {
                await context.Branches.AddAsync(new Branch { Name = b.Name, Address = b.Address });
            }
            await context.SaveChangesAsync();
        }

        var branchesList = await context.Branches.ToListAsync();

        // -------------------------
        // 4️⃣ Faculty
        var facultyList = new List<FacultyProfile>();

        if (!await context.FacultyProfiles.AnyAsync())
        {
            for (int i = 1; i <= 3; i++)
            {
                string email = $"faculty{i}@college.com";
                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
                    await userManager.CreateAsync(user, "Faculty123!");
                    await userManager.AddToRoleAsync(user, "Faculty");
                }

                facultyList.Add(new FacultyProfile
                {
                    IdentityUserId = user.Id,
                    Name = faker.Name.FullName(),
                    Email = email,
                    Phone = faker.Phone.PhoneNumber()
                });
            }

            await context.FacultyProfiles.AddRangeAsync(facultyList);
            await context.SaveChangesAsync();
        }

        facultyList = await context.FacultyProfiles.ToListAsync();

        // -------------------------
        // 5️⃣ Students
        var studentList = new List<StudentProfile>();

        if (!await context.StudentProfiles.AnyAsync())
        {
            for (int i = 1; i <= 6; i++)
            {
                string email = $"student{i}@college.com";
                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
                    await userManager.CreateAsync(user, "Student123!");
                    await userManager.AddToRoleAsync(user, "Student");
                }

                studentList.Add(new StudentProfile
                {
                    IdentityUserId = user.Id,
                    Name = faker.Name.FullName(),
                    Email = email,
                    Phone = faker.Phone.PhoneNumber(),
                    Address = faker.Address.FullAddress(),
                    StudentNumber = 1000 + i
                });
            }

            await context.StudentProfiles.AddRangeAsync(studentList);
            await context.SaveChangesAsync();
        }

        studentList = await context.StudentProfiles.ToListAsync();

        // -------------------------
        // 6️⃣ Courses
        var courses = new List<Course>();

        if (!await context.Courses.AnyAsync())
        {
            string[] courseNames = { "Mathematics", "Physics", "Computer Science", "Business", "English Literature" };

            foreach (var name in courseNames)
            {
                var branch = branchesList[rnd.Next(branchesList.Count)];
                var faculty = facultyList[rnd.Next(facultyList.Count)];

                var course = new Course
                {
                    Name = name,
                    BranchId = branch.Id,
                    FacultyId = faculty.Id,
                    StartDate = faker.Date.Past(),
                    EndDate = faker.Date.Future()
                };

                faculty.Courses.Add(course);
                branch.Courses.Add(course);

                courses.Add(course);
            }

            await context.Courses.AddRangeAsync(courses);
            await context.SaveChangesAsync();
        }

        courses = await context.Courses.Include(c => c.Faculty).Include(c => c.Branch).ToListAsync();

        // -------------------------
        // 7️⃣ Enrolments
        var enrolments = new List<CourseEnrolment>();

        if (!await context.CourseEnrolments.AnyAsync())
        {
            foreach (var course in courses)
            {
                var shuffledStudents = studentList
                    .OrderBy(_ => rnd.Next())
                    .Take(rnd.Next(2, studentList.Count))
                    .ToList();

                foreach (var student in shuffledStudents)
                {
                    CourseEnrolment enroltmp = new CourseEnrolment
                    {
                        CourseId = course.Id,
                        StudentProfileId = student.Id,
                        EnrolDate = DateTime.UtcNow.AddDays(-rnd.Next(30, 90)),
                        Status = eStatus.Active
                    };
                    enrolments.Add(enroltmp);
                    student.CourseEnrolments.Add(enroltmp);
                }
            }

            await context.CourseEnrolments.AddRangeAsync(enrolments);
            await context.SaveChangesAsync();
        }

        enrolments = await context.CourseEnrolments.ToListAsync();

        // -------------------------
        // 8️⃣ Assignments + linked AssignmentResults
        if (!await context.Assignments.AnyAsync())
        {
            foreach (var course in courses)
            {
                for (int i = 1; i <= 3; i++)
                {
                    var assignment = new Assignment
                    {
                        CourseId = course.Id,
                        Title = $"{course.Name} Assignment {i}",
                        MaxScore = 100,
                        DueDate = DateTime.UtcNow.AddDays(-rnd.Next(1, 60))
                    };

                    var enrolledStudents = enrolments.Where(e => e.CourseId == course.Id).ToList();
                    foreach (var enrol in enrolledStudents)
                    {
                        AssignmentResult assigntmp = new AssignmentResult
                        {
                            StudentProfileId = enrol.StudentProfileId,
                            Score = rnd.Next(50, 101),
                            Feedback = faker.Lorem.Sentence(),
                            Assignment = assignment,
                        };
                        assignment.AssignmentResults.Add(assigntmp);
                        if (enrol.StudentProfile != null) 
                        {
                            enrol.StudentProfile.AssignmentResults.Add(assigntmp);
                        }
                    }

                    await context.Assignments.AddAsync(assignment);
                }
            }

            await context.SaveChangesAsync();
        }

        // -------------------------
        // 9️⃣ Exams + Results
        if (!await context.Exams.AnyAsync())
        {
            foreach (var course in courses)
            {
                for (int i = 1; i <= 2; i++)
                {
                    var exam = new Exam
                    {
                        CourseId = course.Id,
                        Title = $"{course.Name} Exam {i}",
                        MaxScore = 100,
                        Date = DateTime.UtcNow.AddDays(-rnd.Next(1, 60)),
                        ResultsReleased = true
                    };

                    await context.Exams.AddAsync(exam);
                    await context.SaveChangesAsync();

                    var enrolledStudents = enrolments.Where(e => e.CourseId == course.Id).ToList();
                    foreach (var enrol in enrolledStudents)
                    {
                        ExamResult examtmp = new ExamResult
                        {
                            ExamId = exam.Id,
                            StudentProfileId = enrol.StudentProfileId,
                            Score = rnd.Next(40, 101),
                            EndDate = DateTime.UtcNow
                        };
                        await context.ExamResults.AddAsync(examtmp);
                        if (enrol.StudentProfile != null)
                        {
                            enrol.StudentProfile.ExamsResults.Add(examtmp);
                        }
                    }
                }
            }

            await context.SaveChangesAsync();
        }

        // -------------------------
        // 🔟 Attendance
        if (!await context.AttendanceRecords.AnyAsync())
        {
            foreach (var enrol in enrolments)
            {
                for (int week = 1; week <= 5; week++)
                {
                    await context.AttendanceRecords.AddAsync(new AttendanceRecord
                    {
                        CourseEnrolmentId = enrol.Id,
                        Date = DateTime.UtcNow.AddDays(-7 * week),
                        Present = rnd.NextDouble() > 0.3
                    });
                }
            }

            await context.SaveChangesAsync();
        }
    }
}