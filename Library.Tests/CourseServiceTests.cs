using Library.Domain;
using Library.MVC.Services;
using System;
using Xunit;

namespace Library.Tests
{
    public class CourseServiceTests
    {
        [Fact]
        public void CreateCourse_WithValidData_CreatesSuccessfully()
        {
            var service = new CourseService();

            var course = service.CreateCourse("CS101", 1,
                new DateTime(2026, 1, 1),
                new DateTime(2026, 6, 1),
                10);

            Assert.NotNull(course);
            Assert.Equal("CS101", course.Name);
            Assert.Equal(1, course.BranchId);
        }

        [Fact]
        public void CreateCourse_WithoutName_ThrowsException()
        {
            var service = new CourseService();

            Assert.Throws<ArgumentException>(() =>
                service.CreateCourse("", 1, DateTime.Now, DateTime.Now.AddDays(1), 1));
        }

        [Fact]
        public void CreateCourse_InvalidDates_ThrowsException()
        {
            var service = new CourseService();

            Assert.Throws<ArgumentException>(() =>
                service.CreateCourse("CS101", 1,
                    DateTime.Now,
                    DateTime.Now.AddDays(-1),
                    1));
        }

        [Fact]
        public void GetCoursesByFaculty_ReturnsCorrectCourses()
        {
            var service = new CourseService();

            service.CreateCourse("C1", 1, DateTime.Now, DateTime.Now.AddDays(10), 1);
            service.CreateCourse("C2", 1, DateTime.Now, DateTime.Now.AddDays(10), 1);
            service.CreateCourse("C3", 1, DateTime.Now, DateTime.Now.AddDays(10), 2);

            var result = service.GetCoursesByFaculty(1);

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetById_ReturnsCorrectCourse()
        {
            var service = new CourseService();

            var course = service.CreateCourse("CS101", 1,
                DateTime.Now,
                DateTime.Now.AddDays(10),
                1);

            var found = service.GetById(course.Id);

            Assert.Equal(course, found);
        }

        [Fact]
        public void GetAll_ReturnsAllCourses()
        {
            var service = new CourseService();

            service.CreateCourse("C1", 1, DateTime.Now, DateTime.Now.AddDays(10), 1);
            service.CreateCourse("C2", 1, DateTime.Now, DateTime.Now.AddDays(10), 1);

            var all = service.GetAll();

            Assert.Equal(2, all.Count);
        }
    }
}