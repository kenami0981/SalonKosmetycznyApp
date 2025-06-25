using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using SalonKosmetycznyApp.Model;

namespace SalonKosmetycznyApp.Services
{
    internal class AppointmentBookingService
    {
            private readonly string _connectionString;

            public AppointmentBookingService()
            {
                _connectionString = System.Configuration.ConfigurationManager
                    .ConnectionStrings["MySqlConnection"].ConnectionString;

                EnsureAppointmentsTableExists();
            }

            private void EnsureAppointmentsTableExists()
            {
                using var conn = new MySqlConnection(_connectionString);
                conn.Open();

                var createTablesQuery = @"
                CREATE TABLE IF NOT EXISTS clients (
                    id INT AUTO_INCREMENT PRIMARY KEY,
                    name VARCHAR(100) NOT NULL,
                    phone VARCHAR(20)
                );

                CREATE TABLE IF NOT EXISTS rooms (
                    id INT AUTO_INCREMENT PRIMARY KEY,
                    name VARCHAR(100) NOT NULL,
                    type VARCHAR(50)
                );

                CREATE TABLE IF NOT EXISTS employees (
                    id INT AUTO_INCREMENT PRIMARY KEY,
                    name VARCHAR(100) NOT NULL
                );

                CREATE TABLE IF NOT EXISTS appointments (
                    id INT AUTO_INCREMENT PRIMARY KEY,
                    client_id INT NOT NULL,
                    treatment_id INT NOT NULL,
                    room_id INT NOT NULL,
                    employee_id INT NOT NULL,
                    appointment_date DATETIME NOT NULL,
                    FOREIGN KEY (client_id) REFERENCES clients(id),
                    FOREIGN KEY (treatment_id) REFERENCES treatments(id),
                    FOREIGN KEY (room_id) REFERENCES rooms(id),
                    FOREIGN KEY (employee_id) REFERENCES employees(id)
                );
            ";

                using var cmd = new MySqlCommand(createTablesQuery, conn);
                cmd.ExecuteNonQuery();
            }


        public List<Client> GetAllClients()
        {
            var clients = new List<Client>();

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand("SELECT id, name, surname, gender, phone, email, note FROM clients", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var client = new Client(
                    reader.GetString("name"),                       // _clientName
                    reader.GetString("surname"),                    // _clientSurname
                    reader.GetString("phone"),                      // _clientNumber
                    reader.IsDBNull(reader.GetOrdinal("gender"))    // _clientGender
                        ? null
                        : reader.GetString("gender"),
                    reader.IsDBNull(reader.GetOrdinal("email"))     // _clientEmail
                        ? null
                        : reader.GetString("email"),
                    reader.IsDBNull(reader.GetOrdinal("note"))      // clientNote
                        ? null
                        : reader.GetString("note")
                )
                {
                    Id = reader.GetInt32("id") // Przypisanie właściwości Id
                };

                clients.Add(client);
            }

            return clients;
        }

        public List<Treatment> GetAllTreatments()
        {
            var treatments = new List<Treatment>();

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            // Używamy kolumny duration_minutes i type (zgodnie z definicją tabeli)
            var cmd = new MySqlCommand("SELECT id, name, description, duration_minutes, price, type FROM treatments", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var description = reader.IsDBNull(reader.GetOrdinal("description")) ? "" : reader.GetString(reader.GetOrdinal("description"));
                var typeString = reader.IsDBNull(reader.GetOrdinal("type")) ? "" : reader.GetString(reader.GetOrdinal("type"));
                TreatmentType type = Enum.TryParse<TreatmentType>(typeString, out var parsedType)
                    ? parsedType
                    : TreatmentType.Twarz;  // domyślny typ, dostosuj jeśli trzeba

                var treatment = new Treatment(
                    reader.GetString(reader.GetOrdinal("name")),
                    description,
                    TimeSpan.FromMinutes(reader.GetInt32(reader.GetOrdinal("duration_minutes"))),
                    reader.GetDecimal(reader.GetOrdinal("price")),
                    type
                )
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id"))
                };

                treatments.Add(treatment);
            }

            return treatments;
        }




        public List<TreatmentRoom> GetAllTreatmentRooms()
        {
            var rooms = new List<TreatmentRoom>();

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand("SELECT ID_Sali, Nazwa, Typ_sali, Dostepna FROM TreatmentRoom", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var room = new TreatmentRoom(
                reader.GetString("Nazwa"),                         // name
                reader.GetString("Typ_sali"),                     // roomType
                reader.GetString("Dostepna")
                )
                {
                    Id = reader.GetInt32("ID_Sali")
                };

                rooms.Add(room);
            }

            return rooms;
        }


        public List<Employee> GetAllEmployees()
        {
            var employees = new List<Employee>();

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand(@"
        SELECT Id, Login, Password, Phone, Email, HireDate, Position, Status, FirstName, LastName FROM Employees", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var hireDate = reader.IsDBNull(reader.GetOrdinal("HireDate"))
                    ? (DateTime?)null
                    : reader.GetDateTime("HireDate");

                var employee = new Employee(
                    reader.GetString("Login"),
                    reader.GetString("Password"),
                    reader.GetString("Phone"),
                    reader.GetString("Email"),
                    hireDate,
                    reader.GetString("Position"),
                    reader.GetString("Status"),
                    reader.GetString("FirstName"),
                    reader.GetString("LastName")
                )
                                {
                                    Id = reader.GetInt32("Id")
                                };

                employees.Add(employee);
            }

            return employees;
        }


        public void AddAppointment(int clientId, int treatmentId, int roomId, int employeeId, DateTime appointmentDate)
            {
                using var conn = new MySqlConnection(_connectionString);
                conn.Open();

                var cmd = new MySqlCommand(@"
                INSERT INTO appointments (client_id, treatment_id, room_id, employee_id, appointment_date)
                VALUES (@clientId, @treatmentId, @roomId, @employeeId, @appointmentDate)", conn);

                cmd.Parameters.AddWithValue("@clientId", clientId);
                cmd.Parameters.AddWithValue("@treatmentId", treatmentId);
                cmd.Parameters.AddWithValue("@roomId", roomId);
                cmd.Parameters.AddWithValue("@employeeId", employeeId);
                cmd.Parameters.AddWithValue("@appointmentDate", appointmentDate);

                cmd.ExecuteNonQuery();
            }

        public List<Reservation> GetAllAppointments()
        {
            var appointments = new List<Reservation>();

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand(@"
        SELECT a.id, c.name AS client_name, t.name AS treatment_name, 
               r.name AS room_name, e.name AS employee_name, a.appointment_date
        FROM appointments a
        JOIN clients c ON a.client_id = c.id
        JOIN treatments t ON a.treatment_id = t.id
        JOIN rooms r ON a.room_id = r.id
        JOIN employees e ON a.employee_id = e.id", conn);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var reservation = new Reservation
                {
                    Id = reader.GetInt32("id"),
                    ClientName = reader.GetString("client_name"),
                    TreatmentName = reader.GetString("treatment_name"),
                    RoomName = reader.GetString("room_name"),
                    EmployeeName = reader.GetString("employee_name"),
                    AppointmentDate = reader.GetDateTime("appointment_date")
                };

                appointments.Add(reservation);
            }

            return appointments;
        }

    }
}