using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonKosmetycznyApp.Model
{
    public class Employee
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime? HireDate { get; set; }
        public string Position { get; set; }
        public string Status { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Employee(string login, string password, string phone, string email, DateTime? hireDate, string position, string status, string firstName, string lastName)
        {
            Login = login;
            Password = password;
            Phone = phone;
            Email = email;
            HireDate = hireDate;
            Position = position;
            Status = status;
            FirstName = firstName;
            LastName = lastName;
        }

        public override string ToString()
        {
            return $"{FirstName}, {LastName}, {Position}, {Phone}";
        }
    }
}