using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonKosmetycznyApp.Model
{
    public class TreatmentRoom
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string RoomType { get; set; }
        public string Availability { get; set; }
        public TreatmentRoom() { } 
        public TreatmentRoom(string name, string roomType, string availability)
        {
            Name = name;
            RoomType = roomType;
            Availability = availability;
        }

        public override string ToString()
        {
            return $"{Name}, {RoomType}, {Availability}";
        }
    }
}

