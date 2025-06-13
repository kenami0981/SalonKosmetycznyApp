using SalonKosmetycznyApp.Commands;
using SalonKosmetycznyApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SalonKosmetycznyApp.ViewModel
{
    internal class AddClientViewModel : BaseViewModel
    {
        public ObservableCollection<Client> Clients { get; } = new ObservableCollection<Client>();
        private string _clientName;
        public string ClientName
        {
            get => _clientName;
            set
            {
                if (_clientName != value)
                {
                    _clientName = value;
                    OnPropertyChanged(nameof(ClientName));
                }

            }
        }
        private string _clientSurname;
        public string ClientSurname
        {
            get => _clientSurname;
            set
            {
                if (_clientSurname != value)
                {
                    _clientSurname = value;
                    OnPropertyChanged(nameof(ClientSurname));
                }

            }
        }
        private string _clientNumber;
        public string ClientNumber
        {
            get => _clientNumber;
            set
            {
                if (_clientNumber != value)
                {
                    _clientNumber = value;
                    OnPropertyChanged(nameof(ClientNumber));
                }

            }
        }
        private string _clientEmail;
        public string ClientEmail
        {
            get => _clientEmail;
            set
            {
                if (_clientEmail != value)
                {
                    _clientEmail = value;
                    OnPropertyChanged(nameof(ClientEmail));
                }

            }
        }
        private string _clientNote;
        public string ClientNote
        {
            get => _clientNote;
            set
            {
                if (_clientNote != value)
                {
                    _clientNote = value;
                    OnPropertyChanged(nameof(ClientNote));
                }

            }
        }
        private ICommand? _addClientCommand;
        public ICommand AddClientCommand
        {
            get
            {
                if (_addClientCommand == null)
                {
                    _addClientCommand = new RelayCommand(
                    (object o) =>
                    {

                            
                        Client client = new Client(_clientName, _clientSurname, _clientNumber, ClientEmail, ClientNote);
                        Clients.Add(client);


                        OnPropertyChanged(nameof(_addClientCommand));
                        ClientName = string.Empty;
                        ClientSurname = string.Empty;
                        ClientNumber = string.Empty;
                        ClientEmail = string.Empty;
                        ClientNote = string.Empty;

                    },
                    (object o) =>
                    {
                        return !string.IsNullOrEmpty(_clientName) &&
                        !string.IsNullOrEmpty(_clientSurname) &&
                        !string.IsNullOrEmpty(_clientNumber);
                    });
                }

                return _addClientCommand;

            }
        }
    }
}
