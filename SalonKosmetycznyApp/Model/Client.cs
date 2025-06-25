using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SalonKosmetycznyApp.Model
{
    public class Client
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public string ClientSurname { get; set; }
        public string ClientGender { get; set; }
        public int ClientNumber { get; set; }
        public string ClientEmail { get; set; }
        public string ClientNote { get; set; }


        public Client(string _clientName, string _clientSurname, int? _clientNumber,string? _clientGender, string? _clientEmail = null, string? clientNote = null)
        {
            ClientName = _clientName;
            ClientSurname = _clientSurname;
            ClientNumber = (int)_clientNumber;
            ClientGender = _clientGender;
            ClientEmail = _clientEmail;
            ClientNote = clientNote;
        }



        public override string ToString()
        {
            //MessageBox.Show(ClientGender.ToString());
            return $"{ClientName}, {ClientSurname}, {ClientNumber} {ClientGender} {ClientEmail} {ClientNote}";
        }
    }
}
