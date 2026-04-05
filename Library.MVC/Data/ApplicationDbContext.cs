using Library.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Serilog.Parsing;

namespace Library.MVC.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets for all domain entities
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<CourseEnrolment> CourseEnrolments => Set<CourseEnrolment>();
    public DbSet<StudentProfile> StudentProfiles => Set<StudentProfile>();
    public DbSet<FacultyProfile> FacultyProfiles => Set<FacultyProfile>();
    public DbSet<Assignment> Assignments => Set<Assignment>();
    public DbSet<AssignmentResult> AssignmentResults => Set<AssignmentResult>();
    public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();
    public DbSet<Exam> Exams => Set<Exam>();
    public DbSet<ExamResult> ExamResults => Set<ExamResult>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ---------------------------
        // Course → Branch
        modelBuilder.Entity<Course>()
            .HasOne(c => c.Branch)
            .WithMany() // Branch does not have list of courses? Add if desired
            .HasForeignKey(c => c.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        // ---------------------------
        // Course → Faculty
        modelBuilder.Entity<Course>()
            .HasOne(c => c.Faculty)
            .WithMany(f => f.Courses)
            .HasForeignKey(c => c.FacultyId)
            .OnDelete(DeleteBehavior.Restrict);

        // ---------------------------
        // CourseEnrolment → Student & Course
        modelBuilder.Entity<CourseEnrolment>()
            .HasOne(ce => ce.StudentProfile)
            .WithMany() // Could add List<CourseEnrolment> in StudentProfile
            .HasForeignKey(ce => ce.StudentProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        // Enum conversion for CourseEnrolment.Status
        modelBuilder.Entity<CourseEnrolment>()
            .Property(ce => ce.Status)
            .HasConversion<string>();

        // ---------------------------
        // Assignment → Course
        modelBuilder.Entity<Assignment>()
            .HasOne(a => a.Course)
            .WithMany() // Could add List<Assignment> in Course if needed
            .HasForeignKey(a => a.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // AssignmentResult → Assignment & Student
        modelBuilder.Entity<AssignmentResult>()
            .HasOne(ar => ar.Assignment)
            .WithMany() // Could add List<AssignmentResult> in Assignment
            .HasForeignKey(ar => ar.AssignmentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AssignmentResult>()
            .HasOne(ar => ar.StudentProfile)
            .WithMany() // Could add List<AssignmentResult> in StudentProfile
            .HasForeignKey(ar => ar.StudentProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        // ---------------------------
        // AttendanceRecord → CourseEnrolment
        modelBuilder.Entity<AttendanceRecord>()
            .HasOne(a => a.CourseEnrolment)
            .WithMany() // Could add List<AttendanceRecord> in CourseEnrolment
            .HasForeignKey(a => a.CourseEnrolmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // ---------------------------
        // Exam → Course
        modelBuilder.Entity<Exam>()
            .HasOne(e => e.Course)
            .WithMany() // Could add List<Exam> in Course
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // ExamResult → Exam & Student
        modelBuilder.Entity<ExamResult>()
            .HasOne(er => er.Exam)
            .WithMany() // Could add List<ExamResult> in Exam
            .HasForeignKey(er => er.ExamId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ExamResult>()
            .HasOne(er => er.StudentProfile)
            .WithMany() // Could add List<ExamResult> in StudentProfile
            .HasForeignKey(er => er.StudentProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}