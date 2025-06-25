using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SalonKosmetycznyApp.Services;
using SalonKosmetycznyApp.Model;

namespace SalonKosmetycznyApp.ViewModel
{
     class AppointmentBookingViewModel: BaseViewModel
    {
            private readonly AppointmentBookingService _service;

            public ObservableCollection<Client> Clients { get; set; } = new ObservableCollection<Client>();

            private Client _selectedClient;
            public Client SelectedClient
            {
                get => _selectedClient;
                set
                {
                    _selectedClient = value;
                    OnPropertyChanged(nameof(SelectedClient));
                }
            }
        public ObservableCollection<Treatment> Treatments { get; set; } = new ObservableCollection<Treatment>();

        private Treatment _selectedTreatment;
        public Treatment SelectedTreatment
        {
            get => _selectedTreatment;
            set
            {
                _selectedTreatment = value;
                OnPropertyChanged(nameof(SelectedTreatment));
            }
        }
        public ObservableCollection<TreatmentRoom> TreatmentRooms { get; set; } = new ObservableCollection<TreatmentRoom>();
        private TreatmentRoom _selectedTreatmentRoom;
        public TreatmentRoom SelectedTreatmentRoom
        {
            get => _selectedTreatmentRoom;
            set
            {
                _selectedTreatmentRoom = value;
                OnPropertyChanged(nameof(SelectedTreatmentRoom));
            }
        }

        public ObservableCollection<Employee> Employees { get; set; } = new ObservableCollection<Employee>();
        private Employee _selectedEmployee;
        public Employee SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                _selectedEmployee = value;
                OnPropertyChanged(nameof(SelectedEmployee));
            }
        }

        public AppointmentBookingViewModel()
            {
                _service = new AppointmentBookingService();
                LoadClients();
                LoadTreatments();
                LoadTreatmentRooms();
                LoadEmployees();
        }

            private void LoadClients()
            {
                var clientsFromDb = _service.GetAllClients();
                Clients.Clear();
                foreach (var client in clientsFromDb)
                {
                    Clients.Add(client);
                }
            }

        private void LoadTreatments()
        {
            var treatmentsFromDb = _service.GetAllTreatments();
            Treatments.Clear();
            foreach (var treatment in treatmentsFromDb)
            {
                Treatments.Add(treatment);
            }
        }
        private void LoadTreatmentRooms()
        {
            var roomsFromDb = _service.GetAllTreatmentRooms();
            TreatmentRooms.Clear();
            foreach (var room in roomsFromDb)
            {
                TreatmentRooms.Add(room);
            }
        }

        private void LoadEmployees()
        {
            var employeesFromDb = _service.GetAllEmployees();
            Employees.Clear();
            foreach (var emp in employeesFromDb)
            {
                Employees.Add(emp);
            }
        }
    }
    }