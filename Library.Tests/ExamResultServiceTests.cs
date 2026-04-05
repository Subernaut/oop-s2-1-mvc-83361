using Library.Domain;
using Library.MVC.Services;
using System;
using Xunit;

namespace Library.Tests
{
    public class ExamResultServiceTests
    {
        [Fact]
        public void CreateResult_WithValidData_CreatesSuccessfully()
        {
            var service = new ExamResultService();
            var exam = new Exam { Id = 1, Title = "Math", MaxScore = 100, ResultsReleased = true };

            var result = service.CreateResult(1, 1, 80, exam);

            Assert.NotNull(result);
            Assert.Equal(80, result.Score);
            Assert.Equal(80, result.Grade);
            Assert.Equal(1, result.StudentProfileId);
            Assert.True(result.Id > 0);
        }

        [Fact]
        public void CreateResult_WithScoreAboveMax_ThrowsException()
        {
            var service = new ExamResultService();
            var exam = new Exam { Id = 1, Title = "Math", MaxScore = 100 };

            Assert.Throws<ArgumentException>(() =>
                service.CreateResult(1, 1, 150, exam));
        }

        [Fact]
        public void CreateResult_WithNegativeScore_ThrowsException()
        {
            var service = new ExamResultService();
            var exam = new Exam { Id = 1, Title = "Math", MaxScore = 100 };

            Assert.Throws<ArgumentException>(() =>
                service.CreateResult(1, 1, -5, exam));
        }

        [Fact]
        public void GetResultById_ReturnsCorrectResult()
        {
            var service = new ExamResultService();
            var exam = new Exam { Id = 1, Title = "Physics", MaxScore = 100 };

            var result = service.CreateResult(1, 2, 70, exam);
            var retrieved = service.GetResultById(result.Id);

            Assert.Equal(result, retrieved);
        }

        [Fact]
        public void GetAllResults_ReturnsAllCreatedResults()
        {
            var service = new ExamResultService();
            var exam = new Exam { Id = 1, Title = "Chemistry", MaxScore = 100 };

            service.CreateResult(1, 1, 60, exam);
            service.CreateResult(1, 2, 90, exam);

            var all = service.GetAllResults();

            Assert.Equal(2, all.Count);
        }

        [Fact]
        public void GetResultsForStudent_WhenResultsReleased_ReturnsResults()
        {
            var service = new ExamResultService();
            var exam = new Exam { Id = 1, MaxScore = 100, ResultsReleased = true };

            service.CreateResult(1, 1, 75, exam);

            var results = service.GetResultsForStudent(1, true);

            Assert.Single(results);
        }

        [Fact]
        public void GetResultsForStudent_WhenResultsNotReleased_ReturnsEmptyForStudent()
        {
            var service = new ExamResultService();
            var exam = new Exam { Id = 1, MaxScore = 100, ResultsReleased = false };

            service.CreateResult(1, 1, 75, exam);

            var results = service.GetResultsForStudent(1, true);

            Assert.Empty(results);
        }

        [Fact]
        public void GetResultsForStudent_WhenFaculty_IgnoresReleaseFlag()
        {
            var service = new ExamResultService();
            var exam = new Exam { Id = 1, MaxScore = 100, ResultsReleased = false };

            service.CreateResult(1, 1, 75, exam);

            var results = service.GetResultsForStudent(1, false);

            Assert.Single(results);
        }
    }
}