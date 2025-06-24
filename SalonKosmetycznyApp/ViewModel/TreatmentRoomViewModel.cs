using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SalonKosmetycznyApp.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SalonKosmetycznyApp.Commands;
using SalonKosmetycznyApp.Model;

namespace SalonKosmetycznyApp.ViewModel
{
    internal class TreatmentRoomViewModel : BaseViewModel
    {
        private readonly TreatmentRoomService _treatmentRoomService;

        public TreatmentRoomViewModel()
        {
            _treatmentRoomService = new TreatmentRoomService();
            TreatmentRooms = new ObservableCollection<TreatmentRoom>();
            LoadData();
            AvailabilityList = new ObservableCollection<string> { "Tak", "Nie" };
            RoomTypeList = new ObservableCollection<string>
        {
            "Masażowa",
            "Kosmetyczna",
            "Manicure/Pedicure",
            "Depilacja",
            "Gabinet laserowy",
            "Gabinet SPA",
            "Sala do makijażu"
        };

            AddTreatmentRoomCommand = new RelayCommand(_ => AddTreatmentRoom(), _ => true);
            UpdateTreatmentRoomCommand = new RelayCommand(_ => UpdateTreatmentRoom(), _ => SelectedTreatmentRoom != null);
            DeleteTreatmentRoomCommand = new RelayCommand(_ => DeleteTreatmentRoom(), _ => SelectedTreatmentRoom != null);

        }

        // Właściwości do powiązania z formularzem
        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        private string _roomType;

        public string RoomType
        {
            get => _roomType;
            set { _roomType = value; OnPropertyChanged(); }
        }


        private string _availability;
        public string Availability
        {
            get => _availability;
            set { _availability = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> AvailabilityList { get; }
        public ObservableCollection<string> RoomTypeList { get; }
      


        // Lista i zaznaczony element
        public ObservableCollection<TreatmentRoom> TreatmentRooms { get; }

        private bool _isClearing = false;
        private TreatmentRoom _selectedTreatmentRoom;
        public TreatmentRoom SelectedTreatmentRoom
        {
            get => _selectedTreatmentRoom;
            set
            {
                _selectedTreatmentRoom = value;
                OnPropertyChanged();

                if (!_isClearing && value != null)
                {
                    Name = value.Name;
                    RoomType = value.RoomType;
                    Availability = value.Availability;
                }
            }
        }

        // Komendy
        public ICommand AddTreatmentRoomCommand { get; }
        public ICommand UpdateTreatmentRoomCommand { get; }
        public ICommand DeleteTreatmentRoomCommand { get; }

        private void AddTreatmentRoom()
        {
            if (string.IsNullOrWhiteSpace(Name) ||
                string.IsNullOrWhiteSpace(Availability) ||
                (Availability != "Tak" && Availability != "Nie"))
            {
                System.Windows.MessageBox.Show(
                    "Uzupełnij wszystkie wymagane pola.",
                    "Błąd walidacji",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Warning);
                return;
            }

            try
            {
                var newRoom = new TreatmentRoom
                {
                    Name = this.Name,
                    RoomType = this.RoomType,
                    Availability = this.Availability
                };

                _treatmentRoomService.AddTreatmentRoom(newRoom);
                LoadData();
                ClearForm();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    "Wystąpił błąd podczas dodawania sali:\n" + ex.Message,
                    "Błąd",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
        }

        

        private void UpdateTreatmentRoom()
        {
            if (SelectedTreatmentRoom == null) return;

            SelectedTreatmentRoom.Name = this.Name;
            SelectedTreatmentRoom.RoomType = this.RoomType;
            SelectedTreatmentRoom.Availability = this.Availability;

            _treatmentRoomService.UpdateTreatmentRoom(SelectedTreatmentRoom);
            LoadData();
            ClearForm();
        }

        private void DeleteTreatmentRoom()
        {
            if (SelectedTreatmentRoom == null) return;

            _treatmentRoomService.DeleteTreatmentRoom(SelectedTreatmentRoom.Id);
            LoadData();
            ClearForm();
        }
        public void LoadData()
        {
            TreatmentRooms.Clear();
            var roomsFromDb = _treatmentRoomService.GetAllTreatmentRooms();
            foreach (var room in roomsFromDb)
            {
                TreatmentRooms.Add(room);
            }
        }

        private void ClearForm()
        {
            Name = string.Empty;
            RoomType = string.Empty;
            Availability = null;
            SelectedTreatmentRoom = null;
            _isClearing = false;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

