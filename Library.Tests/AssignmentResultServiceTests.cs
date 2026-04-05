using Library.Domain;
using Library.MVC.Services;
using System;
using Xunit;

namespace Library.Tests
{
    public class AssignmentResultServiceTests
    {
        [Fact]
        public void AddResult_WithValidData_CreatesSuccessfully()
        {
            var service = new AssignmentResultService();
            var assignment = new Assignment { Id = 1, MaxScore = 100 };

            var result = service.AddResult(1, 1, 80, assignment);

            Assert.NotNull(result);
            Assert.Equal(80, result.Score);
        }

        [Fact]
        public void AddResult_InvalidScore_ThrowsException()
        {
            var service = new AssignmentResultService();
            var assignment = new Assignment { Id = 1, MaxScore = 100 };

            Assert.Throws<ArgumentException>(() =>
                service.AddResult(1, 1, 150, assignment));
        }

        [Fact]
        public void AddResult_Duplicate_ThrowsException()
        {
            var service = new AssignmentResultService();
            var assignment = new Assignment { Id = 1, MaxScore = 100 };

            service.AddResult(1, 1, 80, assignment);

            Assert.Throws<ArgumentException>(() =>
                service.AddResult(1, 1, 90, assignment));
        }

        [Fact]
        public void GetResultsByStudent_ReturnsCorrectResults()
        {
            var service = new AssignmentResultService();
            var assignment = new Assignment { Id = 1, MaxScore = 100 };

            service.AddResult(1, 1, 80, assignment);
            service.AddResult(1, 2, 90, assignment);

            var results = service.GetResultsByStudent(1);

            Assert.Single(results);
        }

        [Fact]
        public void GetResultsByAssignment_ReturnsCorrectResults()
        {
            var service = new AssignmentResultService();
            var assignment = new Assignment { Id = 1, MaxScore = 100 };

            service.AddResult(1, 1, 80, assignment);
            service.AddResult(1, 2, 90, assignment);

            var results = service.GetResultsByAssignment(1);

            Assert.Equal(2, results.Count);
        }

        [Fact]
        public void GetResultsByStudent_NoResults_ReturnsEmpty()
        {
            var service = new AssignmentResultService();

            var results = service.GetResultsByStudent(999);

            Assert.Empty(results);
        }
    }
}