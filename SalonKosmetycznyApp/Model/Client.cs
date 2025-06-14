using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonKosmetycznyApp.Model
{
    public class Client
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public string ClientSurname { get; set; }
        //public string ClientSex { get; set; }
        //public DateTime ClientBirthDate { get; set; }
        public string ClientNumber { get; set; }
        public string ClientEmail { get; set; }
        public string ClientNote { get; set; }


        public Client(string _clientName, string _clientSurname, string _clientNumber, string? _clientEmail = null, string? clientNote = null)
        {
            ClientName = _clientName;
            ClientSurname = _clientSurname;
            ClientNumber = _clientNumber;
            ClientEmail = _clientEmail;
            ClientNote = clientNote;
        }



        public override string ToString()
        {
            return $"{ClientName}, {ClientSurname}, {ClientNumber} {ClientEmail} {ClientNote}";
        }
    }
}
