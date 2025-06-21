using MySql.Data.MySqlClient;
using SalonKosmetycznyApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonKosmetycznyApp.Services
{
    public class ProductService
    {
        private readonly string _connectionString;

        public ProductService()
        {
            _connectionString = System.Configuration.ConfigurationManager
                .ConnectionStrings["MySqlConnection"].ConnectionString;

            EnsureTableExists();
        }

        private void EnsureTableExists()
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand(@"
                CREATE TABLE IF NOT EXISTS products (
                    id INT AUTO_INCREMENT PRIMARY KEY,
                    name VARCHAR(100) NOT NULL,
                    description TEXT,
                    price DECIMAL(10,2) NOT NULL,
                    productStock INT NOT NULL
                );
            ", conn);

            cmd.ExecuteNonQuery();
        }

        public List<Product> GetAllProducts()
        {
            var products = new List<Product>();

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand("SELECT id, name, description, price, productStock FROM products", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var description = reader.IsDBNull(reader.GetOrdinal("description")) ? "" : reader.GetString("description");

                var product = new Product(
                    reader.GetString("name"),
                    description,
                    reader.GetDecimal("price"),
                    reader.GetInt32("productStock")
                )
                {
                    Id = reader.GetInt32("id")
                };

                products.Add(product);
            }

            return products;
        }

        public void AddProduct(Product product)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand(@"
                INSERT INTO products (name, description, price, productStock) 
                VALUES (@name, @desc, @price, @stock)", conn);

            cmd.Parameters.AddWithValue("@name", product.Name);
            cmd.Parameters.AddWithValue("@desc", product.Description);
            cmd.Parameters.AddWithValue("@price", product.Price);
            cmd.Parameters.AddWithValue("@stock", product.ProductStock);

            cmd.ExecuteNonQuery();
        }

        public void UpdateProduct(Product product)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand(@"
                UPDATE products 
                SET name = @name, description = @desc, price = @price, productStock = @stock 
                WHERE id = @id", conn);

            cmd.Parameters.AddWithValue("@name", product.Name);
            cmd.Parameters.AddWithValue("@desc", product.Description);
            cmd.Parameters.AddWithValue("@price", product.Price);
            cmd.Parameters.AddWithValue("@stock", product.ProductStock);
            cmd.Parameters.AddWithValue("@id", product.Id);

            cmd.ExecuteNonQuery();
        }

        public void DeleteProduct(int productId)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand("DELETE FROM products WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", productId);

            cmd.ExecuteNonQuery();
        }
    }
}
