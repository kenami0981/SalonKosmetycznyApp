using SalonKosmetycznyApp.Commands;
using SalonKosmetycznyApp.Model;
using SalonKosmetycznyApp.Services; 
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace SalonKosmetycznyApp.ViewModel
{
    internal class AddClientViewModel : BaseViewModel
    {
        public AddClientViewModel()
        {
            LoadData();
        }
        private readonly ClientService _clientService = new ClientService();
        public List<string> GenderOptions { get; } = new() { "Kobieta", "Mężczyzna" };

        public ObservableCollection<Client> Clients { get; } = new ObservableCollection<Client>();

        private ICollectionView _clientsView;
        public ICollectionView ClientsView
        {
            get => _clientsView;
            private set
            {
                _clientsView = value;
                OnPropertyChanged(nameof(ClientsView));
            }
        }

        private string _searchTerm;
        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                if (_searchTerm != value)
                {
                    _searchTerm = value;
                    OnPropertyChanged(nameof(SearchTerm));
                    ClientsView?.Refresh();
                }
            }
        }

        public void LoadData()
        {
            Clients.Clear();
            var clientsFromDb = _clientService.GetAllClients();
            foreach (var client in clientsFromDb)
                Clients.Add(client);

            InitializeClientsView();
        }
        public void ClearForm()
        {
            ClientName = ClientSurname = ClientNumber = ClientEmail = ClientNote = string.Empty;
            ClientGender = null;
            SelectedClient = null;
        }

        public void InitializeClientsView()
        {
            ClientsView = CollectionViewSource.GetDefaultView(Clients);
            ClientsView.Filter = FilterClients;
        }

        private bool FilterClients(object obj)
        {
            if (obj is Client client)
            {
                if (string.IsNullOrWhiteSpace(SearchTerm)) return true;

                var term = SearchTerm.ToLower();
                return client.ClientName.ToLower().Contains(term)
                    || client.ClientSurname.ToLower().Contains(term)
                    || (client.ClientEmail?.ToLower().Contains(term) ?? false)
                    || client.ClientNumber.ToLower().Contains(term);
            }
            return false;
        }

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
        private string _clientGender;
        public string ClientGender
        {
            get => _clientGender;
            set => SetProperty(ref _clientGender, value);
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
        private Client _selectedClient;
        public Client? SelectedClient
        {
            get => _selectedClient;
            set
            {
                _selectedClient = value;
                if (_selectedClient != null)
                {
                    ClientName = _selectedClient.ClientName;
                    ClientSurname = _selectedClient.ClientSurname;
                    ClientNumber = _selectedClient.ClientNumber;
                    ClientEmail = _selectedClient.ClientEmail;
                    ClientNote = _selectedClient.ClientNote;

                    OnPropertyChanged(nameof(SelectedClient));
                }
            }
        }

        private ICommand _addClientCommand;
        public ICommand AddClientCommand => _addClientCommand ??= new RelayCommand(
            o =>
            {
                var client = new Client(ClientName, ClientSurname, ClientNumber, ClientGender, ClientEmail, ClientNote);
                _clientService.AddClient(client);
                LoadData();
                ClearForm();
                OnPropertyChanged(nameof(AddClientCommand));
            },
            o => !string.IsNullOrWhiteSpace(ClientName) &&
                 !string.IsNullOrWhiteSpace(ClientSurname) &&
                 !string.IsNullOrWhiteSpace(ClientNumber)
        );

        private ICommand _updateClientCommand;
        public ICommand UpdateClientCommand => _updateClientCommand ??= new RelayCommand(
            o =>
            {
                if (_selectedClient != null)
                {
                    _selectedClient.ClientName = ClientName;
                    _selectedClient.ClientSurname = ClientSurname;
                    _selectedClient.ClientNumber = ClientNumber;
                    _selectedClient.ClientGender = ClientGender;
                    _selectedClient.ClientEmail = ClientEmail;
                    _selectedClient.ClientNote = ClientNote;

                    _clientService.UpdateClient(_selectedClient);
                    LoadData();
                    ClearForm();
                }
            },
            o => _selectedClient != null &&
                 !string.IsNullOrWhiteSpace(ClientName) &&
                 !string.IsNullOrWhiteSpace(ClientSurname) &&
                 !string.IsNullOrWhiteSpace(ClientNumber)
        );

        private ICommand _deleteClientCommand;
        public ICommand DeleteClientCommand => _deleteClientCommand ??= new RelayCommand(
            o =>
            {
                if (_selectedClient != null)
                {
                    _clientService.DeleteClient(_selectedClient.Id);
                    Clients.Remove(_selectedClient);
                    LoadData();
                    ClearForm();
                }
            },
            o => _selectedClient != null
        );

        

        protected bool SetProperty<T>(ref T field, T value, string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName ?? nameof(field));
                return true;
            }
            return false;
        }
    }
}
