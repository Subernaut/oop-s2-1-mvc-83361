using Library.Domain;
using Library.MVC.Services;
using System;
using Xunit;

namespace Library.Tests
{
    public class AttendanceRecordServiceTests
    {
        [Fact]
        public void AddAttendance_WithValidData_CreatesSuccessfully()
        {
            var service = new AttendanceRecordService();

            var record = service.AddAttendance(1, new DateTime(2026, 4, 1), true);

            Assert.NotNull(record);
            Assert.Equal(1, record.CourseEnrolmentId);
            Assert.True(record.Present);
            Assert.True(record.Id > 0);
        }

        [Fact]
        public void AddAttendance_DuplicateSameDate_ThrowsException()
        {
            var service = new AttendanceRecordService();

            service.AddAttendance(1, new DateTime(2026, 4, 1), true);

            Assert.Throws<ArgumentException>(() =>
                service.AddAttendance(1, new DateTime(2026, 4, 1), false));
        }

        [Fact]
        public void AddAttendance_InvalidEnrolment_ThrowsException()
        {
            var service = new AttendanceRecordService();

            Assert.Throws<ArgumentException>(() =>
                service.AddAttendance(0, DateTime.Now, true));
        }

        [Fact]
        public void GetAttendanceByEnrolment_ReturnsCorrectRecords()
        {
            var service = new AttendanceRecordService();

            service.AddAttendance(1, new DateTime(2026, 4, 1), true);
            service.AddAttendance(1, new DateTime(2026, 4, 2), false);
            service.AddAttendance(2, new DateTime(2026, 4, 1), true);

            var records = service.GetAttendanceByEnrolment(1);

            Assert.Equal(2, records.Count);
        }

        [Fact]
        public void GetAttendanceByEnrolment_ReturnsOrderedByDate()
        {
            var service = new AttendanceRecordService();

            service.AddAttendance(1, new DateTime(2026, 4, 2), true);
            service.AddAttendance(1, new DateTime(2026, 4, 1), true);

            var records = service.GetAttendanceByEnrolment(1);

            Assert.True(records[0].Date < records[1].Date);
        }
        [Fact]
        public void AddAttendance_DifferentEnrolmentsSameDate_AllowsBoth()
        {
            var service = new AttendanceRecordService();

            service.AddAttendance(1, new DateTime(2026, 4, 1), true);
            service.AddAttendance(2, new DateTime(2026, 4, 1), true);

            var records1 = service.GetAttendanceByEnrolment(1);
            var records2 = service.GetAttendanceByEnrolment(2);

            Assert.Single(records1);
            Assert.Single(records2);
        }

        [Fact]
        public void GetAttendanceByEnrolment_NoRecords_ReturnsEmpty()
        {
            var service = new AttendanceRecordService();

            var records = service.GetAttendanceByEnrolment(999);

            Assert.Empty(records);
        }
    }
}