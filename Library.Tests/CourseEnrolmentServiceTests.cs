using Library.Domain;
using Library.MVC.Services;
using System;
using Xunit;

namespace Library.Tests
{
    public class CourseEnrolmentServiceTests
    {
        [Fact]
        public void EnrolStudent_WithValidData_CreatesSuccessfully()
        {
            var service = new CourseEnrolmentService();

            var enrolment = service.EnrolStudent(1, 1);

            Assert.NotNull(enrolment);
            Assert.Equal(1, enrolment.StudentProfileId);
            Assert.Equal(1, enrolment.CourseId);
            Assert.Equal(eStatus.Active, enrolment.Status);
        }

        [Fact]
        public void EnrolStudent_DuplicateEnrollment_ThrowsException()
        {
            var service = new CourseEnrolmentService();

            service.EnrolStudent(1, 1);

            Assert.Throws<ArgumentException>(() =>
                service.EnrolStudent(1, 1));
        }

        [Fact]
        public void EnrolStudent_InvalidStudent_ThrowsException()
        {
            var service = new CourseEnrolmentService();

            Assert.Throws<ArgumentException>(() =>
                service.EnrolStudent(0, 1));
        }

        [Fact]
        public void GetEnrolmentsByStudent_ReturnsCorrectData()
        {
            var service = new CourseEnrolmentService();

            service.EnrolStudent(1, 1);
            service.EnrolStudent(1, 2);
            service.EnrolStudent(2, 1);

            var result = service.GetEnrolmentsByStudent(1);

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void DeactivateEnrolment_ChangesStatusToInactive()
        {
            var service = new CourseEnrolmentService();

            var enrolment = service.EnrolStudent(1, 1);

            service.DeactivateEnrolment(enrolment.Id);

            Assert.Equal(eStatus.Inactive, enrolment.Status);
        }
    }
}