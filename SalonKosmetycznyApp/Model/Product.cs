using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonKosmetycznyApp.Model
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int ProductStock {  get; set; }

        public Product(string name, string description, decimal price, int productStock)
        {
            Name = name;
            Description = description;
            Price = price;
            ProductStock = productStock;
        }

        public override string ToString()
        {
            return $"{Name} ({ProductStock} szt.) - {Price:C}";
        }
    }

}
