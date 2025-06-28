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

                CREATE TABLE IF NOT EXISTS treatmentroom (  -- Używamy 'treatmentroom' zamiast 'rooms'
        ID_Sali INT AUTO_INCREMENT PRIMARY KEY,
        Nazwa VARCHAR(50) NOT NULL,
        Typ_sali VARCHAR(50),
        Dostepna ENUM('Tak', 'Nie') NOT NULL
    );

                CREATE TABLE IF NOT EXISTS employees (
                    id INT AUTO_INCREMENT PRIMARY KEY,
                    name VARCHAR(100) NOT NULL
                );
                CREATE TABLE IF NOT EXISTS appointments (
                    id INT AUTO_INCREMENT PRIMARY KEY,
                    client_id INT NOT NULL,
                    treatment_id INT NOT NULL,
                    room_id INT NOT NULL,  -- Powiązanie z 'treatmentroom'
                    employee_id INT NOT NULL,
                    appointment_date DATETIME NOT NULL,
                    FOREIGN KEY (client_id) REFERENCES clients(id),
                    FOREIGN KEY (treatment_id) REFERENCES treatments(id),
                    FOREIGN KEY (room_id) REFERENCES treatmentroom(ID_Sali),  -- Zmieniamy 'rooms' na 'treatmentroom'
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
                // Pobieranie i parsowanie numeru telefonu
                string phoneString = reader.IsDBNull(reader.GetOrdinal("phone")) ? null : reader.GetString("phone");
                int? clientNumber = null;

                if (!string.IsNullOrEmpty(phoneString))
                {
                    if (int.TryParse(phoneString, out int parsedNumber))
                    {
                        clientNumber = parsedNumber;
                    }
                    else
                    {
                        // Możesz dodać tutaj logowanie błędu, jeśli numer nie jest poprawny
                        clientNumber = null;
                    }
                }

                var client = new Client(
                    reader.GetString("name"),                      // _clientName
                    reader.GetString("surname"),                   // _clientSurname
                    clientNumber,                                  // _clientNumber
                    reader.IsDBNull(reader.GetOrdinal("gender"))   // _clientGender
                        ? null
                        : reader.GetString("gender"),
                    reader.IsDBNull(reader.GetOrdinal("email"))    // _clientEmail
                        ? null
                        : reader.GetString("email"),
                    reader.IsDBNull(reader.GetOrdinal("note"))     // clientNote
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

            var cmd = new MySqlCommand("SELECT ID_Sali, Nazwa, Typ_sali, Dostepna FROM treatmentroom", conn);
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
                    : reader.GetDateTime(reader.GetOrdinal("HireDate"));

                int? phoneNumber = null;
                if (!reader.IsDBNull(reader.GetOrdinal("Phone")))
                {
                    phoneNumber = reader.GetInt32(reader.GetOrdinal("Phone"));
                }


                var employee = new Employee(
                    reader.GetString(reader.GetOrdinal("Login")),
                    reader.GetString(reader.GetOrdinal("Password")),
                    phoneNumber, // tutaj używasz sparsowanego phoneNumber
                    reader.GetString(reader.GetOrdinal("Email")),
                    hireDate,
                    reader.GetString(reader.GetOrdinal("Position")),
                    reader.GetString(reader.GetOrdinal("Status")),
                    reader.GetString(reader.GetOrdinal("FirstName")),
                    reader.GetString(reader.GetOrdinal("LastName"))
                )
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id"))
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
        public void AddAppointment(Reservation reservation)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            // Zmieniamy 'rooms' na 'treatmentroom' w zapytaniu SQL
            var cmd = new MySqlCommand(@"
        INSERT INTO appointments (client_id, treatment_id, room_id, employee_id, appointment_date)
        VALUES (@clientId, @treatmentId, @roomId, @employeeId, @appointmentDate)", conn);

            // Dodajemy parametry
            cmd.Parameters.AddWithValue("@clientId", reservation.Client.Id);  // Wartość z obiektu Client
            cmd.Parameters.AddWithValue("@treatmentId", reservation.Treatment.Id);  // Wartość z obiektu Treatment
            cmd.Parameters.AddWithValue("@roomId", reservation.TreatmentRoom.Id);  // Wartość z obiektu TreatmentRoom
            cmd.Parameters.AddWithValue("@employeeId", reservation.Employee.Id);  // Wartość z obiektu Employee
            cmd.Parameters.AddWithValue("@appointmentDate", reservation.AppointmentDate);  // Wartość z obiektu Reservation

            // Wykonaj zapytanie
            cmd.ExecuteNonQuery();
        }

        //        public List<Reservation> GetAllAppointments()
        //        {
        //            var appointments = new List<Reservation>();

        //            using var conn = new MySqlConnection(_connectionString);
        //            conn.Open();

        //            var cmd = new MySqlCommand(@"
        //            SELECT a.id, c.name AS client_name, t.name AS treatment_name, 
        //                   r.id AS room_id, r.name AS room_name, r.type AS room_type, 
        //                   e.name AS employee_name, a.appointment_date
        //            FROM appointments a
        //            JOIN clients c ON a.client_id = c.id
        //            JOIN treatments t ON a.treatment_id = t.id
        //JOIN treatmentroom tr ON a.room_id = tr.ID_Sali
        //            JOIN employees e ON a.employee_id = e.id", conn);

        //            using var reader = cmd.ExecuteReader();

        //            while (reader.Read())
        //            {
        //                var reservation = new Reservation
        //                {
        //                    Id = reader.GetInt32("id"),
        //                    Client = new Client(
        //                        reader.GetString("client_name"),  // clientName
        //                        reader.GetString("client_surname"),  // clientSurname
        //                        reader.GetString("client_number"),  // clientNumber
        //                        reader.IsDBNull(reader.GetOrdinal("client_gender")) ? null : reader.GetString("client_gender"),  // clientGender
        //                        reader.IsDBNull(reader.GetOrdinal("client_email")) ? null : reader.GetString("client_email"),  // clientEmail
        //                        reader.IsDBNull(reader.GetOrdinal("client_note")) ? null : reader.GetString("client_note")  // clientNote
        //                    ),
        //                    Treatment = new Treatment(
        //                        reader.GetString("treatment_name"), // name
        //                        reader.IsDBNull(reader.GetOrdinal("treatment_description")) ? "" : reader.GetString("treatment_description"), // description
        //                        TimeSpan.FromMinutes(reader.GetInt32("treatment_duration_minutes")), // duration
        //                        reader.GetDecimal("treatment_price"), // price
        //                        Enum.TryParse<TreatmentType>(reader.GetString("treatment_type"), out var treatmentType) ? treatmentType : TreatmentType.Twarz // type
        //                    ),
        //                    TreatmentRoom = new TreatmentRoom
        //                    {
        //                        Id = reader.GetInt32("room_id"),
        //                        Name = reader.GetString("room_name"),
        //                        RoomType = reader.GetString("room_type")
        //                    },
        //                    EmployeeName = reader.GetString("employee_name"),
        //                    AppointmentDate = reader.GetDateTime("appointment_date")
        //                };

        //                appointments.Add(reservation);
        //            }



        //            return appointments;
        //        }

        public List<Reservation> GetAllAppointments()
        {
            var appointments = new List<Reservation>();

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand(@"
        SELECT 
            a.id, a.appointment_date,
            c.id AS client_id, c.name AS client_name, c.surname AS client_surname, 
            c.gender AS client_gender, c.phone AS client_number, 
            c.email AS client_email, c.note AS client_note,

            t.id AS treatment_id, t.name AS treatment_name, 
            t.description AS treatment_description, 
            t.duration_minutes AS treatment_duration_minutes, 
            t.price AS treatment_price, t.type AS treatment_type,

            r.ID_Sali AS room_id, r.Nazwa AS room_name, r.Typ_sali AS room_type,

            e.Id AS employee_id, e.FirstName, e.LastName

        FROM appointments a
        JOIN clients c ON a.client_id = c.id
        JOIN treatments t ON a.treatment_id = t.id
        JOIN treatmentroom r ON a.room_id = r.ID_Sali
        JOIN employees e ON a.employee_id = e.id", conn);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {

                int? clientPhoneNumber = null;
                if (!reader.IsDBNull(reader.GetOrdinal("client_number")))
                {
                    var phoneStr = reader.GetString(reader.GetOrdinal("client_number"));
                    if (int.TryParse(phoneStr, out int parsedPhone))
                    {
                        clientPhoneNumber = parsedPhone;
                    }
                    else
                    {
                        // Możesz logować błąd parsowania lub ustawić domyślną wartość
                        clientPhoneNumber = 0; // Domyślna wartość
                    }
                }

                var reservation = new Reservation
                {
                    Id = reader.GetInt32("id"),
                    AppointmentDate = reader.GetDateTime("appointment_date"),

                    Client = new Client(
                        reader.GetString("client_name"),
                        reader.GetString("client_surname"),
                        clientPhoneNumber,
                        reader.IsDBNull(reader.GetOrdinal("client_gender")) ? null : reader.GetString("client_gender"),
                        reader.IsDBNull(reader.GetOrdinal("client_email")) ? null : reader.GetString("client_email"),
                        reader.IsDBNull(reader.GetOrdinal("client_note")) ? null : reader.GetString("client_note")
                    )
                    {
                        Id = reader.GetInt32("client_id")
                    },

                    Treatment = new Treatment(
                        reader.GetString("treatment_name"),
                        reader.IsDBNull(reader.GetOrdinal("treatment_description")) ? "" : reader.GetString("treatment_description"),
                        TimeSpan.FromMinutes(reader.GetInt32("treatment_duration_minutes")),
                        reader.GetDecimal("treatment_price"),
                        Enum.TryParse<TreatmentType>(reader.GetString("treatment_type"), out var treatmentType) ? treatmentType : TreatmentType.Twarz
                    )
                    {
                        Id = reader.GetInt32("treatment_id")
                    },

                    TreatmentRoom = new TreatmentRoom
                    {
                        Id = reader.GetInt32("room_id"),
                        Name = reader.GetString("room_name"),
                        RoomType = reader.GetString("room_type")
                    },

                    Employee = new Employee(
                        login: "", password: "", phone: 0, email: "",
                        hireDate: null, position: "", status: "",
                        firstName: reader.GetString("FirstName"),
                        lastName: reader.GetString("LastName")
                    )
                    {
                        Id = reader.GetInt32("employee_id")
                    }
                };

                appointments.Add(reservation);
            }

            return appointments;
        }
        public void DeleteAppointment(int reservationId)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand("DELETE FROM appointments WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", reservationId);
            cmd.ExecuteNonQuery();
        }

        public void UpdateAppointment(Reservation reservation)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand(@"
        UPDATE appointments
        SET client_id = @clientId,
            treatment_id = @treatmentId,
            room_id = @roomId,
            employee_id = @employeeId,
            appointment_date = @appointmentDate
        WHERE id = @id", conn);

            cmd.Parameters.AddWithValue("@clientId", reservation.Client.Id);
            cmd.Parameters.AddWithValue("@treatmentId", reservation.Treatment.Id);
            cmd.Parameters.AddWithValue("@roomId", reservation.TreatmentRoom.Id);
            cmd.Parameters.AddWithValue("@employeeId", reservation.Employee.Id);
            cmd.Parameters.AddWithValue("@appointmentDate", reservation.AppointmentDate);
            cmd.Parameters.AddWithValue("@id", reservation.Id);

            cmd.ExecuteNonQuery();
        }




    }
}