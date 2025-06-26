using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using SalonKosmetycznyApp.Model;

namespace SalonKosmetycznyApp.Services
{
    public class TreatmentRoomService
    {
        private readonly string _connectionString;

        public TreatmentRoomService()
        {
            _connectionString = System.Configuration.ConfigurationManager
                .ConnectionStrings["MySqlConnection"].ConnectionString;
            EnsureTreatmentRoomTableExists();
        }

        public void EnsureTreatmentRoomTableExists()
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS TreatmentRoom (
                    ID_Sali INT AUTO_INCREMENT PRIMARY KEY,
                    Nazwa VARCHAR(50) NOT NULL,
                    Typ_sali VARCHAR(50),
                    Dostepna ENUM('Tak', 'Nie') NOT NULL
                );";

            using var cmd = new MySqlCommand(createTableQuery, conn);
            cmd.ExecuteNonQuery();
        }

        public List<TreatmentRoom> GetAllTreatmentRooms()
        {
            var rooms = new List<TreatmentRoom>();

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand("SELECT * FROM TreatmentRoom", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var id = reader.GetInt32("ID_Sali");
                var name = reader.GetString("Nazwa");
                var type = reader.IsDBNull(reader.GetOrdinal("Typ_sali")) ? string.Empty : reader.GetString("Typ_sali");
                var available = reader.GetString("Dostepna");

                var room = new TreatmentRoom(name, type, available)
                {
                    Id = id
                };

                rooms.Add(room);
            }

            return rooms;
        }

        public void AddTreatmentRoom(TreatmentRoom room)
        {
            // Validate Availability
            if (room.Availability != "Tak" && room.Availability != "Nie")
            {
                throw new ArgumentException("Availability must be either 'Tak' or 'Nie'.");
            }

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand(@"
                INSERT INTO TreatmentRoom (Nazwa, Typ_sali, Dostepna)
                VALUES (@name, @type, @availability)", conn);

            cmd.Parameters.AddWithValue("@name", room.Name ?? string.Empty);
            cmd.Parameters.AddWithValue("@type", room.RoomType ?? string.Empty);
            cmd.Parameters.AddWithValue("@availability", room.Availability);

            cmd.ExecuteNonQuery();
        }

        public void UpdateTreatmentRoom(TreatmentRoom room)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand(@"
                UPDATE TreatmentRoom SET 
                    Nazwa = @name,
                    Typ_sali = @type,
                    Dostepna = @availability
                WHERE ID_Sali = @id", conn);

            cmd.Parameters.AddWithValue("@id", room.Id);
            cmd.Parameters.AddWithValue("@name", room.Name ?? string.Empty);
            cmd.Parameters.AddWithValue("@type", room.RoomType ?? string.Empty);
            cmd.Parameters.AddWithValue("@availability", room.Availability ?? "Nie");

            cmd.ExecuteNonQuery();
        }

        public void DeleteTreatmentRoom(int roomId)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand("DELETE FROM TreatmentRoom WHERE ID_Sali = @id", conn);
            cmd.Parameters.AddWithValue("@id", roomId);

            cmd.ExecuteNonQuery();
        }
    }
}

