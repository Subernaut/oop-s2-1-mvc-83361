using Library.Domain;
using Library.MVC.Services;
using Xunit;

namespace Library.Tests
{
    public class FacultyProfileServiceTests
    {
        [Fact]
        public void CreateFaculty_WithValidData_CreatesSuccessfully()
        {
            var service = new FacultyProfileService();
            var faculty = service.CreateFaculty("Dr. John Doe", "john@test.com", "5551234567");

            Assert.NotNull(faculty);
            Assert.Equal("Dr. John Doe", faculty.Name);
            Assert.Equal("john@test.com", faculty.Email);
            Assert.Equal("5551234567", faculty.Phone);
            Assert.True(faculty.Id > 0);
        }

        [Fact]
        public void CreateFaculty_WithoutName_ThrowsException()
        {
            var service = new FacultyProfileService();
            Assert.Throws<ArgumentException>(() => service.CreateFaculty("", "john@test.com", "5551234567"));
        }

        [Fact]
        public void CreateFaculty_WithoutEmail_ThrowsException()
        {
            var service = new FacultyProfileService();
            Assert.Throws<ArgumentException>(() => service.CreateFaculty("John", "", "5551234567"));
        }

        [Fact]
        public void CreateFaculty_WithoutPhone_ThrowsException()
        {
            var service = new FacultyProfileService();
            Assert.Throws<ArgumentException>(() => service.CreateFaculty("John", "john@test.com", ""));
        }

        [Fact]
        public void GetFacultyById_ReturnsCorrectFaculty()
        {
            var service = new FacultyProfileService();
            var faculty = service.CreateFaculty("Dr. Smith", "smith@test.com", "5559876543");

            var retrieved = service.GetFacultyById(faculty.Id);

            Assert.Equal(faculty, retrieved);
        }

        [Fact]
        public void GetAllFaculty_ReturnsAllCreatedFaculty()
        {
            var service = new FacultyProfileService();
            service.CreateFaculty("Dr. Alice", "alice@test.com", "5551112222");
            service.CreateFaculty("Dr. Bob", "bob@test.com", "5553334444");

            var allFaculty = service.GetAllFaculty();

            Assert.Equal(2, allFaculty.Count);
        }

        [Fact]
        public void AssignCourse_AddsCourseToFaculty()
        {
            var service = new FacultyProfileService();
            var faculty = service.CreateFaculty("Dr. Jane", "jane@test.com", "5556667777");
            var course = new Course { Id = 1, Name = "Math 101" };

            service.AssignCourse(faculty.Id, course);

            Assert.Contains(course, faculty.Courses);
        }

        [Fact]
        public void RemoveCourse_RemovesCourseFromFaculty()
        {
            var service = new FacultyProfileService();
            var faculty = service.CreateFaculty("Dr. Jane", "jane@test.com", "5556667777");
            var course = new Course { Id = 1, Name = "Math 101" };

            service.AssignCourse(faculty.Id, course);
            service.RemoveCourse(faculty.Id, course);

            Assert.DoesNotContain(course, faculty.Courses);
        }
    }
}