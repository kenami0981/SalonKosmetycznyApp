using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using SalonKosmetycznyApp.Model;

namespace SalonKosmetycznyApp.Services
{
    public class EmployeeService
    {
        private readonly string _connectionString;

        public EmployeeService()
        {
            _connectionString = System.Configuration.ConfigurationManager
                .ConnectionStrings["MySqlConnection"].ConnectionString;
            EnsureEmployeesTableExists();
        }

        public void EnsureEmployeesTableExists()
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Employees (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    Login VARCHAR(50) NOT NULL,
                    Password VARCHAR(50) NOT NULL,
                    Phone INT,
                    Email VARCHAR(100),
                    HireDate DATE,
                    Position VARCHAR(50),
                    Status VARCHAR(20),
                    FirstName VARCHAR(50) NOT NULL,
                    LastName VARCHAR(50) NOT NULL
                );";

            using var cmd = new MySqlCommand(createTableQuery, conn);
            cmd.ExecuteNonQuery();
        }

        public List<Employee> GetAllEmployees()
        {
            var employees = new List<Employee>();

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand("SELECT * FROM Employees", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var login = reader.IsDBNull(reader.GetOrdinal("Login")) ? string.Empty : reader.GetString("Login");
                var password = reader.IsDBNull(reader.GetOrdinal("Password")) ? string.Empty : reader.GetString("Password");
                var phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? 0 : reader.GetInt32(reader.GetOrdinal("Phone"));
                var email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString("Email");
                var hireDate = reader.IsDBNull(reader.GetOrdinal("HireDate")) ? (DateTime?)null : reader.GetDateTime("HireDate");
                var position = reader.IsDBNull(reader.GetOrdinal("Position")) ? string.Empty : reader.GetString("Position");
                var status = reader.IsDBNull(reader.GetOrdinal("Status")) ? string.Empty : reader.GetString("Status");
                var firstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? string.Empty : reader.GetString("FirstName");
                var lastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? string.Empty : reader.GetString("LastName");
                var id = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32("Id");

                var employee = new Employee(
                    login,
                    password,
                    phone,
                    email,
                    hireDate,
                    position,
                    status,
                    firstName,
                    lastName
                )
                {
                    Id = id
                };

                employees.Add(employee);
            }

            return employees;
        }


        public void AddEmployee(Employee employee)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand(@"
                INSERT INTO Employees (Login, Password, Phone, Email, HireDate, Position, Status, FirstName, LastName) 
                VALUES (@login, @password, @phone, @email, @hireDate, @position, @status, @firstName, @lastName)", conn);

            cmd.Parameters.AddWithValue("@login", employee.Login ?? string.Empty);
            cmd.Parameters.AddWithValue("@password", employee.Password ?? string.Empty);
            cmd.Parameters.AddWithValue("@phone", (object?)employee.Phone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@email", (object?)employee.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@hireDate", (object?)employee.HireDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@position", employee.Position ?? string.Empty);
            cmd.Parameters.AddWithValue("@status", employee.Status ?? string.Empty);
            cmd.Parameters.AddWithValue("@firstName", employee.FirstName ?? string.Empty);
            cmd.Parameters.AddWithValue("@lastName", employee.LastName ?? string.Empty);

            cmd.ExecuteNonQuery();
        }

        public void UpdateEmployee(Employee employee)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand(@"
                UPDATE Employees SET 
                    Login = @login,
                    Password = @password,
                    Phone = @phone,
                    Email = @email,
                    HireDate = @hireDate,
                    Position = @position,
                    Status = @status,
                    FirstName = @firstName,
                    LastName = @lastName 
                WHERE Id = @id", conn);

            cmd.Parameters.AddWithValue("@id", employee.Id);
            cmd.Parameters.AddWithValue("@login", employee.Login ?? string.Empty);
            cmd.Parameters.AddWithValue("@password", employee.Password ?? string.Empty);
            cmd.Parameters.AddWithValue("@phone", (object?)employee.Phone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@email", (object?)employee.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@hireDate", (object?)employee.HireDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@position", employee.Position ?? string.Empty);
            cmd.Parameters.AddWithValue("@status", employee.Status ?? string.Empty);
            cmd.Parameters.AddWithValue("@firstName", employee.FirstName ?? string.Empty);
            cmd.Parameters.AddWithValue("@lastName", employee.LastName ?? string.Empty);

            cmd.ExecuteNonQuery();
        }

        public void DeleteEmployee(int employeeId)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand("DELETE FROM Employees WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@id", employeeId);

            cmd.ExecuteNonQuery();
        }
    }
}
