using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SalonKosmetycznyApp.Model;

namespace SalonKosmetycznyApp.Services
{
    public class TreatmentService
    {
        private readonly string _connectionString;

        public TreatmentService()
        {
            _connectionString = System.Configuration.ConfigurationManager
                .ConnectionStrings["MySqlConnection"].ConnectionString;

            EnsureTreatmentsTableExists();
        }

        private void EnsureTreatmentsTableExists()
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS treatments (
                    id INT AUTO_INCREMENT PRIMARY KEY,
                    name VARCHAR(100) NOT NULL,
                    description TEXT,
                    duration_minutes INT NOT NULL,
                    price DECIMAL(10,2) NOT NULL,
                    type VARCHAR(100)
                );
            ";

            using var cmd = new MySqlCommand(createTableQuery, conn);
            cmd.ExecuteNonQuery();
        }

        public List<Treatment> GetAllTreatments()
        {
            var treatments = new List<Treatment>();

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand("SELECT id, name, description, duration_minutes, price, type FROM treatments", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var description = reader.IsDBNull(reader.GetOrdinal("description")) ? "" : reader.GetString(reader.GetOrdinal("description"));

                var typeString = reader.IsDBNull(reader.GetOrdinal("type")) ? "" : reader.GetString(reader.GetOrdinal("type"));
                TreatmentType type = Enum.TryParse<TreatmentType>(typeString, out var parsedType)
                    ? parsedType
                    : TreatmentType.Twarz;

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

        public void AddTreatment(Treatment treatment)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand(@"
                INSERT INTO treatments (name, description, duration_minutes, price, type) 
                VALUES (@name, @desc, @duration, @price, @type)", conn);

            cmd.Parameters.AddWithValue("@name", treatment.Name);
            cmd.Parameters.AddWithValue("@desc", treatment.Description);
            cmd.Parameters.AddWithValue("@duration", (int)treatment.Duration.TotalMinutes);
            cmd.Parameters.AddWithValue("@price", treatment.Price);
            cmd.Parameters.AddWithValue("@type", treatment.TreatmentType);

            cmd.ExecuteNonQuery();
        }

        public void UpdateTreatment(Treatment treatment)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand(@"
                UPDATE treatments 
                SET name = @name, description = @desc, duration_minutes = @duration, price = @price, type = @type 
                WHERE id = @id", conn);

            cmd.Parameters.AddWithValue("@name", treatment.Name);
            cmd.Parameters.AddWithValue("@desc", treatment.Description);
            cmd.Parameters.AddWithValue("@duration", (int)treatment.Duration.TotalMinutes);
            cmd.Parameters.AddWithValue("@price", treatment.Price);
            cmd.Parameters.AddWithValue("@type", treatment.TreatmentType.ToString());
            cmd.Parameters.AddWithValue("@id", treatment.Id);

            cmd.ExecuteNonQuery();
        }

        public void DeleteTreatment(int treatmentId)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand("DELETE FROM treatments WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", treatmentId);

            cmd.ExecuteNonQuery();
        }
    }
}
