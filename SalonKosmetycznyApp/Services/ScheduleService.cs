using MySql.Data.MySqlClient;
using SalonKosmetycznyApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonKosmetycznyApp.Services
{
    class ScheduleService
    {
        private readonly string _connectionString;

        public ScheduleService()
        {
            _connectionString = System.Configuration.ConfigurationManager
                .ConnectionStrings["MySqlConnection"].ConnectionString;
            EnsureSchedulesTableExists();
        }
        public void EnsureSchedulesTableExists()
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            string createTableQuery = @"
        CREATE TABLE IF NOT EXISTS schedules (
            id INT AUTO_INCREMENT PRIMARY KEY,
            employeeId INT NOT NULL,
            employeeName VARCHAR(100) NOT NULL,
            startDate DATE NOT NULL,
            startTime TIME NOT NULL,
            endTime TIME NOT NULL,
            CONSTRAINT fk_employee FOREIGN KEY (employeeId) REFERENCES employees(id)
                ON DELETE CASCADE
                ON UPDATE CASCADE
        );";

            using var cmd = new MySqlCommand(createTableQuery, conn);
            cmd.ExecuteNonQuery();
        }



        public List<Schedule> GetAllSchedules()
        {
            var schedules = new List<Schedule>();

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand(
                "SELECT id, employeeId, employeeName, startDate, startTime, endTime FROM schedules", conn);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var schedule = new Schedule(
                    reader.GetInt32("employeeId"),
                    reader.GetString("employeeName"),
                    reader.GetDateTime("startDate"),
                    reader.GetTimeSpan("startTime"),
                    reader.GetTimeSpan("endTime"))
                {
                    Id = reader.GetInt32("id")
                };

                schedules.Add(schedule);
            }

            return schedules;
        }

        public void AddSchedule(Schedule schedule)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand(
    @"INSERT INTO schedules (employeeId, employeeName, startDate, startTime, endTime) 
      VALUES (@employeeId, @employeeName, @startDate, @startTime, @endTime)", conn);
            cmd.Parameters.AddWithValue("@employeeId", schedule.EmployeeId);
            cmd.Parameters.AddWithValue("@employeeName", schedule.EmployeeName);
            cmd.Parameters.AddWithValue("@startDate", schedule.StartDate.Date);
            cmd.Parameters.AddWithValue("@startTime", schedule.StartTime);
            cmd.Parameters.AddWithValue("@endTime", schedule.EndTime);

            cmd.ExecuteNonQuery();
        }

        public void UpdateSchedule(Schedule schedule)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand(
                @"UPDATE schedules SET 
                    employeeName = @employeeName, 
                    startDate = @startDate, 
                    startTime = @startTime, 
                    endTime = @endTime 
                  WHERE id = @id", conn);

            cmd.Parameters.AddWithValue("@employeeName", schedule.EmployeeName);
            cmd.Parameters.AddWithValue("@startDate", schedule.StartDate.Date);
            cmd.Parameters.AddWithValue("@startTime", schedule.StartTime);
            cmd.Parameters.AddWithValue("@endTime", schedule.EndTime);
            cmd.Parameters.AddWithValue("@id", schedule.Id);

            cmd.ExecuteNonQuery();
        }

        public void DeleteSchedule(int id)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand("DELETE FROM schedules WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();
        }

    }
}
