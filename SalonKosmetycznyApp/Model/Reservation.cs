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
        public string ClientName { get; set; }
        public string TreatmentName { get; set; }
        public string RoomName { get; set; }
        public string EmployeeName { get; set; }
        public DateTime AppointmentDate { get; set; }

        // Konstruktor domyślny - przydatny np. do deserializacji lub gdy chcesz ustawiać właściwości osobno
        public Reservation()
        {
        }

        // Konstruktor parametryczny - do szybkiego tworzenia w pełni wypełnionego obiektu
        public Reservation(int id, string clientName, string treatmentName, string roomName, string employeeName, DateTime appointmentDate)
        {
            Id = id;
            ClientName = clientName;
            TreatmentName = treatmentName;
            RoomName = roomName;
            EmployeeName = employeeName;
            AppointmentDate = appointmentDate;
        }

        public override string ToString()
        {
            return $"{ClientName} - {TreatmentName} in {RoomName} with {EmployeeName} on {AppointmentDate}";
        }
    }
}

