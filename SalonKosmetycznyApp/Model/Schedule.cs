using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonKosmetycznyApp.Model
{
    class Schedule
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime StartDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }


        public Schedule(int employeeId, string employeeName, DateTime startDate, TimeSpan startTime, TimeSpan endTime)
        {
            EmployeeId = employeeId;
            EmployeeName = employeeName;
            StartDate = startDate;
            StartTime = startTime;
            EndTime = endTime;
        }



        public override string ToString()
        {
            return $"{EmployeeName}, {StartDate}, {StartTime}, {EndTime}";
        }
    }
}
