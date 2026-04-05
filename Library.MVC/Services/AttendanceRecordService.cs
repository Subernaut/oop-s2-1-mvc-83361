using Library.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.MVC.Services
{
    public class AttendanceRecordService
    {
        private readonly List<AttendanceRecord> _records = new();
        private int _nextId = 1;

        public AttendanceRecord AddAttendance(int enrolmentId, DateTime date, bool present)
        {
            if (enrolmentId <= 0)
                throw new ArgumentException("Invalid enrolment.");

            if (_records.Any(r => r.CourseEnrolmentId == enrolmentId && r.Date.Date == date.Date))
                throw new ArgumentException("Attendance already recorded for this date.");

            var record = new AttendanceRecord
            {
                Id = _nextId++,
                CourseEnrolmentId = enrolmentId,
                Date = date,
                Present = present
            };

            _records.Add(record);
            return record;
        }

        public List<AttendanceRecord> GetAttendanceByEnrolment(int enrolmentId)
        {
            return _records
                .Where(r => r.CourseEnrolmentId == enrolmentId)
                .OrderBy(r => r.Date)
                .ToList();
        }
    }
}