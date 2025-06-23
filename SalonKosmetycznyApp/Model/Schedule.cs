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
        public string EmployeeName { get; set; }
        public DateTime StartDate { get; set; }        // Tylko data, np. 2025-06-16
        public TimeSpan StartTime { get; set; }        // Tylko godzina, np. 08:30
        public TimeSpan EndTime { get; set; }


        public Schedule(string employeeName, DateTime startDate, TimeSpan startTime, TimeSpan endTime)
        {
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
