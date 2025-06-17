using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonKosmetycznyApp.Model
{
    public class Treatment
    {
        public int Id { get; set; }                    
        public string Name { get; set; }               
        public string Description { get; set; }        
        public TimeSpan Duration { get; set; }         
        public decimal Price { get; set; }
        public TreatmentType TreatmentType { get; set; }

        public Treatment(string name, string description, TimeSpan duration, decimal price, TreatmentType type)
        {
            Name = name;
            Description = description;
            Duration = duration;
            Price = price;
            TreatmentType = type;
        }

        public override string ToString()
        {
            return $"{Name} ({Duration.TotalMinutes} min) - {Price:C}";
        }

    }


}
