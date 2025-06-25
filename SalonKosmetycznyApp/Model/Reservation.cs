using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonKosmetycznyApp.Model
{
    public class Reservation
    {
        public int Id { get; set; }
        public Client Client { get; set; }  // Obiekt Client zamiast ClientName
        public Treatment Treatment { get; set; }  // Obiekt Treatment zamiast TreatmentName
        public TreatmentRoom TreatmentRoom { get; set; }  // Obiekt TreatmentRoom zamiast RoomName
        public Employee Employee { get; set; }  // Obiekt Employee zamiast EmployeeName
        public DateTime AppointmentDate { get; set; }
        public string EmployeeName { get; internal set; }


        // Konstruktor domyślny
        public Reservation()
        {
        }

        // Konstruktor parametryczny
        public Reservation(int id, Client client, Treatment treatment, TreatmentRoom treatmentRoom, Employee employee, DateTime appointmentDate)
        {
            Id = id;
            Client = client;
            Treatment = treatment;
            TreatmentRoom = treatmentRoom;
            Employee = employee;
            AppointmentDate = appointmentDate;
        }

        public override string ToString()
        {
            return $"{Client.ClientName} - {Treatment.Name} in {TreatmentRoom.Name} with {Employee.FirstName} on {AppointmentDate}";
        }
    }
}

