using Library.Domain;
using Library.MVC.Services;
using System;
using Xunit;

namespace Library.Tests
{
    public class AssignmentServiceTests
    {
        [Fact]
        public void CreateAssignment_WithValidData_CreatesSuccessfully()
        {
            var service = new AssignmentService();

            var assignment = service.CreateAssignment(1, "Homework 1", 100, DateTime.Now);

            Assert.NotNull(assignment);
            Assert.Equal("Homework 1", assignment.Title);
            Assert.Equal(100, assignment.MaxScore);
        }

        [Fact]
        public void CreateAssignment_WithoutTitle_ThrowsException()
        {
            var service = new AssignmentService();

            Assert.Throws<ArgumentException>(() =>
                service.CreateAssignment(1, "", 100, DateTime.Now));
        }

        [Fact]
        public void CreateAssignment_InvalidMaxScore_ThrowsException()
        {
            var service = new AssignmentService();

            Assert.Throws<ArgumentException>(() =>
                service.CreateAssignment(1, "HW", 0, DateTime.Now));
        }

        [Fact]
        public void GetAssignmentsByCourse_ReturnsCorrectAssignments()
        {
            var service = new AssignmentService();

            service.CreateAssignment(1, "A1", 100, DateTime.Now);
            service.CreateAssignment(1, "A2", 100, DateTime.Now);
            service.CreateAssignment(2, "A3", 100, DateTime.Now);

            var result = service.GetAssignmentsByCourse(1);

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetById_ReturnsCorrectAssignment()
        {
            var service = new AssignmentService();

            var assignment = service.CreateAssignment(1, "A1", 100, DateTime.Now);

            var found = service.GetById(assignment.Id);

            Assert.Equal(assignment, found);
        }

        [Fact]
        public void GetById_InvalidId_ReturnsNull()
        {
            var service = new AssignmentService();

            var result = service.GetById(999);

            Assert.Null(result);
        }
    }
}