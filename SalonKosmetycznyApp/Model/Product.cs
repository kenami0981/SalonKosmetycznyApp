using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonKosmetycznyApp.Model
{
    class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public int ProductStock {  get; set; }

    }

    public Product(int id, string name, string description, float price, int productStock)
        {
            Id = id;    
            Name = name;
            Description = description;
            Price = price;
            ProductStock = productStock;
        }
}
