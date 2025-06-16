using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using SalonKosmetycznyApp.Model;
using System.Windows;


namespace SalonKosmetycznyApp.Services
{

    public class ClientService
    {
        private readonly string _connectionString;

        public ClientService()
        {
            _connectionString = System.Configuration.ConfigurationManager
                .ConnectionStrings["MySqlConnection"].ConnectionString;
            EnsureClientsTableExists();
        }
        public void EnsureClientsTableExists()
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            string createTableQuery = @"
        CREATE TABLE IF NOT EXISTS clients (
            id INT AUTO_INCREMENT PRIMARY KEY,
            name VARCHAR(100) NOT NULL,         
            surname VARCHAR(100) NOT NULL,
            phone VARCHAR(20) NOT NULL,
            gender VARCHAR(15),
            email VARCHAR(100),
            note TEXT
        );
    ";

            using var cmd = new MySqlCommand(createTableQuery, conn);
            cmd.ExecuteNonQuery();
        }
        public List<Client> GetAllClients()
        {
            var clients = new List<Client>();

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            var cmd = new MySqlCommand("SELECT id, name, surname,gender ,phone, email, note FROM clients", conn);


            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var gender = reader.IsDBNull(reader.GetOrdinal("gender")) ? null : reader.GetString("gender");
                var email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString("email");
                var note = reader.IsDBNull(reader.GetOrdinal("note")) ? null : reader.GetString("note");

                var client = new Client(
                    reader.GetString(reader.GetOrdinal("name")),
                    reader.GetString(reader.GetOrdinal("surname")),
                    reader.GetString(reader.GetOrdinal("phone")),
                    gender,
                    email,
                    note
                )
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id"))
                };

                clients.Add(client);
            }

            return clients;
        }

        public void AddClient(Client client)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            var cmd = new MySqlCommand("INSERT INTO clients (name, surname, phone,gender, email, note) VALUES (@name, @surname, @phone, @gender, @email, @note)", conn);

            cmd.Parameters.AddWithValue("@name", client.ClientName);
            cmd.Parameters.AddWithValue("@surname", client.ClientSurname);
            cmd.Parameters.AddWithValue("@phone", client.ClientNumber);
            cmd.Parameters.AddWithValue("@gender", client.ClientGender);
            cmd.Parameters.AddWithValue("@email", client.ClientEmail);
            cmd.Parameters.AddWithValue("@note", client.ClientNote);

            cmd.ExecuteNonQuery();
        }

        public void UpdateClient(Client client)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            var cmd = new MySqlCommand("UPDATE clients SET name=@name, surname=@surname, phone=@phone, gender=@gender,email=@mail, note=@note WHERE id=@id", conn);

            cmd.Parameters.AddWithValue("@name", client.ClientName);
            cmd.Parameters.AddWithValue("@surname", client.ClientSurname);
            cmd.Parameters.AddWithValue("@phone", client.ClientNumber);
            cmd.Parameters.AddWithValue("@gender", client.ClientGender);
            cmd.Parameters.AddWithValue("@mail", client.ClientEmail);
            cmd.Parameters.AddWithValue("@note", client.ClientNote);
            cmd.Parameters.AddWithValue("@id", client.Id);

            cmd.ExecuteNonQuery();
        }

        public void DeleteClient(int clientId)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            var cmd = new MySqlCommand("DELETE FROM clients WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@id", clientId);
            cmd.ExecuteNonQuery();
        }

    }


}
