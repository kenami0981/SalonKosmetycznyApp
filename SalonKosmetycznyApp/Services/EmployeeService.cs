using SalonKosmetycznyApp.Commands;
using SalonKosmetycznyApp.Model;
using SalonKosmetycznyApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace SalonKosmetycznyApp.Services
{
    public class EmployeeService
    {
        private readonly List<Employee> _employees = new List<Employee>();
        private int _nextId = 1;

        public List<Employee> GetAllEmployees()
        {
            return _employees;
        }

        public void AddEmployee(Employee employee)
        {
            employee.Id = _nextId++;
            _employees.Add(employee);
        }

        public void UpdateEmployee(Employee employee)
        {
            var existing = _employees.FirstOrDefault(e => e.Id == employee.Id);
            if (existing != null)
            {
                existing.Login = employee.Login;
                existing.Password = employee.Password;
                existing.Phone = employee.Phone;
                existing.Email = employee.Email;
                existing.HireDate = employee.HireDate;
                existing.Position = employee.Position;
                existing.Status = employee.Status;
                existing.FirstName = employee.FirstName;
                existing.LastName = employee.LastName;
            }
        }

        public void DeleteEmployee(int employeeId)
        {
            var employee = _employees.FirstOrDefault(e => e.Id == employeeId);
            if (employee != null)
            {
                _employees.Remove(employee);
            }
        }
    }
}