using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SalonKosmetycznyApp.Services;
using SalonKosmetycznyApp.Model;
using System.Windows.Input;
using SalonKosmetycznyApp.Commands;
using System.Windows;
using MySql.Data.MySqlClient;

namespace SalonKosmetycznyApp.ViewModel
{
     class AppointmentBookingViewModel: BaseViewModel
    {
            private readonly AppointmentBookingService _service;
        public ObservableCollection<Reservation> Reservations { get; set; } = new ObservableCollection<Reservation>();


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

        private Reservation _selectedReservation;
        public Reservation SelectedReservation
        {
            get => _selectedReservation;
            set
            {
                _selectedReservation = value;
                OnPropertyChanged(nameof(SelectedReservation));

                if (_selectedReservation != null)
                {
                    SelectedClient = Clients.FirstOrDefault(c => c.Id == _selectedReservation.Client.Id);
                    SelectedTreatment = Treatments.FirstOrDefault(t => t.Id == _selectedReservation.Treatment.Id);
                    SelectedDate = _selectedReservation.AppointmentDate.Date;
                    SelectedHour = _selectedReservation.AppointmentDate.ToString("HH:mm");
                    SelectedTreatmentRoom = TreatmentRooms.FirstOrDefault(r => r.Id == _selectedReservation.TreatmentRoom.Id);
                    SelectedEmployee = Employees.FirstOrDefault(e => e.Id == _selectedReservation.Employee.Id);
                }
            }
        }

        public ObservableCollection<Schedule> WorkSchedule { get; set; } = new ObservableCollection<Schedule>();



        private DateTime? _selectedDate;
        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                OnPropertyChanged(nameof(SelectedDate));
            }
        }

        private readonly TreatmentService _treatmentService;
        private readonly ProductService _productService;


        // Komenda do dodania rezerwacji
        public ICommand AddReservationCommand { get; set; }
        public ICommand DeleteReservationCommand { get; }
        public ICommand UpdateReservationCommand { get; }





        public AppointmentBookingViewModel()
            {
                _service = new AppointmentBookingService();
                LoadClients();
                LoadTreatments();
                LoadTreatmentRooms();
                LoadEmployees();
            AddReservationCommand = new RelayCommand(AddReservation, CanAddReservation);
            Reservations = new ObservableCollection<Reservation>(_service.GetAllAppointments());
            DeleteReservationCommand = new RelayCommand(DeleteReservation, CanDeleteReservation);
            UpdateReservationCommand = new RelayCommand(UpdateReservation, CanUpdateReservation);
            _treatmentService = new TreatmentService();
            _productService = new ProductService();





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

            Console.WriteLine($"Loaded {TreatmentRooms.Count} treatment rooms.Lala");
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


        private void AddReservation(object parameter)
        {
            // Walidacja przed dodaniem rezerwacji
            if (SelectedClient == null || SelectedTreatment == null || !SelectedDate.HasValue || SelectedTreatmentRoom == null || SelectedEmployee == null)
            {
                MessageBox.Show("Proszę uzupełnić wszystkie pola przed dodaniem rezerwacji.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Połączenie daty i godziny w jeden string
            string combined = SelectedDate.Value.ToString("yyyy-MM-dd") + " " + SelectedHour;

            if (!DateTime.TryParse(combined, out DateTime appointmentDateTime))
            {
                MessageBox.Show("Nieprawidłowy format daty lub godziny.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Sprawdzenie dostępności pracownika
            if (!IsEmployeeAvailable(SelectedEmployee, appointmentDateTime))
            {
                MessageBox.Show("Pracownik jest niedostępny w wybranym czasie.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Tworzenie nowej rezerwacji
            var newReservation = new Reservation
            {
                Client = SelectedClient,
                Treatment = SelectedTreatment,
                AppointmentDate = appointmentDateTime,
                TreatmentRoom = SelectedTreatmentRoom,
                Employee = SelectedEmployee
            };

            Reservations.Add(newReservation);
            _service.AddAppointment(newReservation);

            WorkSchedule.Add(new Schedule(
                $"{SelectedEmployee.FirstName} {SelectedEmployee.LastName}",
                appointmentDateTime.Date,
                appointmentDateTime.TimeOfDay,
                appointmentDateTime.TimeOfDay + TimeSpan.FromHours(1)
            ));

            MessageBox.Show("Rezerwacja została dodana.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);

            // Czyszczenie formularza:
            SelectedClient = null;
            SelectedTreatment = null;
            SelectedDate = null;
            SelectedHour = null;
            SelectedTreatmentRoom = null;
            SelectedEmployee = null;
        }
        private bool CanAddReservation(object parameter)
        {
            // Sprawdzenie, czy wszystkie wymagane dane są dostępne:
            bool isClientSelected = SelectedClient != null;
            bool isTreatmentSelected = SelectedTreatment != null;
            bool isDateSelected = SelectedDate.HasValue;  // Sprawdzenie, czy data jest wybrana
            bool isRoomSelected = SelectedTreatmentRoom != null && SelectedTreatmentRoom.Availability == "Tak";  // Sprawdzenie, czy pokój jest dostępny
            bool isEmployeeSelected = SelectedEmployee != null && SelectedEmployee.Status == "Aktywny";  // Sprawdzenie, czy pracownik jest aktywny


            //if (SelectedEmployee != null && SelectedDate.HasValue && DateTime.TryParse($"{SelectedDate:yyyy-MM-dd} {SelectedHour}", out DateTime appointmentDateTime))
            //{
            //    bool isEmployeeAvailable = IsEmployeeAvailable(SelectedEmployee, appointmentDateTime);
            //    if (!isEmployeeAvailable)
            //    {
            //        MessageBox.Show("Pracownik jest niedostępny w wybranym czasie.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
            //        return false;
            //    }
            //}

            // Zwróć true, jeśli wszystkie warunki są spełnione, w przeciwnym razie false
            //return true;
            return isClientSelected && isTreatmentSelected && isDateSelected && isRoomSelected && isEmployeeSelected;
        }

        private string _selectedHour;
        public string SelectedHour
        {
            get => _selectedHour;
            set
            {
                _selectedHour = value;
                OnPropertyChanged(nameof(SelectedHour));
            }
        }

        private bool IsEmployeeAvailable(Employee employee, DateTime appointmentDateTime)
        {
            if (employee == null) return false;

            // Pobierz grafik pracownika na dany dzień
            var scheduleForEmployee = WorkSchedule.Where(s =>
                s.EmployeeName == $"{employee.FirstName} {employee.LastName}" &&
                s.StartDate.Date == appointmentDateTime.Date).ToList();

            Console.WriteLine($"Grafik dla pracownika {employee.FirstName} {employee.LastName}:");
            foreach (var schedule in scheduleForEmployee)
            {
                Console.WriteLine($" - Od {schedule.StartTime} do {schedule.EndTime}");
            }

            // Sprawdź, czy w tym czasie pracownik jest zaplanowany
            bool isScheduled = scheduleForEmployee.Any(s =>
                appointmentDateTime.TimeOfDay >= s.StartTime &&
                appointmentDateTime.TimeOfDay < s.EndTime);

            if (!isScheduled)
            {
                Console.WriteLine($"Pracownik {employee.FirstName} {employee.LastName} nie jest zaplanowany na {appointmentDateTime.TimeOfDay}");
            }

            // Sprawdź, czy w tym czasie pracownik ma już rezerwację
            bool hasReservation = Reservations.Any(r =>
                r.Employee.Id == employee.Id &&
                r.AppointmentDate.Date == appointmentDateTime.Date &&
                r.AppointmentDate.TimeOfDay == appointmentDateTime.TimeOfDay);

            if (hasReservation)
            {
                Console.WriteLine($"Pracownik {employee.FirstName} {employee.LastName} ma już rezerwację na {appointmentDateTime.TimeOfDay}");
            }

            return isScheduled && !hasReservation;
        }



        private bool CanDeleteReservation(object parameter)
        {
            return SelectedReservation != null;
        }

        private void DeleteReservation(object parameter)
        {
            if (SelectedReservation == null)
            {
                MessageBox.Show("Proszę wybrać rezerwację do usunięcia.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Potwierdzenie
            var result = MessageBox.Show("Czy na pewno chcesz usunąć rezerwację?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes)
                return;

            _service.DeleteAppointment(SelectedReservation.Id);
            Reservations.Remove(SelectedReservation);
            MessageBox.Show("Rezerwacja została usunięta.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);

            // Czyszczenie formularza
            SelectedReservation = null;
            SelectedClient = null;
            SelectedTreatment = null;
            SelectedDate = null;
            SelectedHour = null;
            SelectedTreatmentRoom = null;
            SelectedEmployee = null;
        }

        private bool CanUpdateReservation(object parameter)
        {
            return SelectedReservation != null;
        }
        private void UpdateReservation(object parameter)
        {
            if (SelectedReservation == null)
            {
                MessageBox.Show("Proszę wybrać rezerwację do edycji.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Połączenie daty i godziny w jeden DateTime
            if (!SelectedDate.HasValue || string.IsNullOrEmpty(SelectedHour))
            {
                MessageBox.Show("Proszę wybrać poprawną datę i godzinę.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string combined = SelectedDate.Value.ToString("yyyy-MM-dd") + " " + SelectedHour;
            if (!DateTime.TryParse(combined, out DateTime appointmentDateTime))
            {
                MessageBox.Show("Nieprawidłowy format daty lub godziny.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Aktualizacja obiektu SelectedReservation:
            SelectedReservation.Client = SelectedClient;
            SelectedReservation.Treatment = SelectedTreatment;
            SelectedReservation.AppointmentDate = appointmentDateTime;
            SelectedReservation.TreatmentRoom = SelectedTreatmentRoom;
            SelectedReservation.Employee = SelectedEmployee;

            try
            {
                _service.UpdateAppointment(SelectedReservation);

                // Odśwież listę rezerwacji:
                var updatedList = _service.GetAllAppointments();
                Reservations.Clear();
                foreach (var r in updatedList)
                    Reservations.Add(r);

                MessageBox.Show("Rezerwacja została zaktualizowana.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);

                // Opcjonalne czyszczenie formularza:
                SelectedReservation = null;
                SelectedClient = null;
                SelectedTreatment = null;
                SelectedDate = null;
                SelectedHour = null;
                SelectedTreatmentRoom = null;
                SelectedEmployee = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas aktualizacji rezerwacji: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //public List<Product> GetProductsForTreatment(int treatmentId)
        //{
        //    var products = new List<Product>();

        //    using var conn = new MySqlConnection(_connectionString);
        //    conn.Open();

        //    var cmd = new MySqlCommand(@"
        //SELECT p.id, p.name, p.description, p.price, p.productStock 
        //FROM products p
        //JOIN treatment_products tp ON p.id = tp.product_id
        //WHERE tp.treatment_id = @treatmentId", conn);

        //    cmd.Parameters.AddWithValue("@treatmentId", treatmentId);

        //    using var reader = cmd.ExecuteReader();
        //    while (reader.Read())
        //    {
        //        products.Add(new Product(
        //            reader.GetString("name"),
        //            reader.GetString("description"),
        //            reader.GetDecimal("price"),
        //            reader.GetInt32("productStock"))
        //        {
        //            Id = reader.GetInt32("id")
        //        });
        //    }

        //    return products;
        //}






    }
}