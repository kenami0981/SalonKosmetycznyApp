using SalonKosmetycznyApp.Commands;
using SalonKosmetycznyApp.Model;
using SalonKosmetycznyApp.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SalonKosmetycznyApp.ViewModel
{
    internal class TreatmentRoomViewModel : BaseViewModel
    {
        private readonly TreatmentRoomService _service;

        private string _name;
        private string _roomType;
        private string _availability;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(nameof(Name)); }
        }

        public string RoomType
        {
            get => _roomType;
            set { _roomType = value; OnPropertyChanged(nameof(RoomType)); }
        }

        public string Availability
        {
            get => _availability;
            set { _availability = value; OnPropertyChanged(nameof(Availability)); }
        }

        public ObservableCollection<string> RoomTypeList { get; }
        public ObservableCollection<string> AvailabilityList { get; }

        public ObservableCollection<TreatmentRoom> TreatmentRooms { get; set; } = new ObservableCollection<TreatmentRoom>();

        private TreatmentRoom _selectedTreatmentRoom;
        public TreatmentRoom SelectedTreatmentRoom
        {
            get => _selectedTreatmentRoom;
            set
            {
                _selectedTreatmentRoom = value;
                OnPropertyChanged(nameof(SelectedTreatmentRoom));

                if (_selectedTreatmentRoom != null)
                {
                    Name = _selectedTreatmentRoom.Name;
                    RoomType = _selectedTreatmentRoom.RoomType;
                    Availability = _selectedTreatmentRoom.Availability;
                }
                else
                {
                   ClearForm();
                }

                CommandManager.InvalidateRequerySuggested();
            }
        }
        public ICommand AddTreatmentRoomCommand { get; }
        public ICommand UpdateTreatmentRoomCommand { get; }
        public ICommand DeleteTreatmentRoomCommand { get; }

        public TreatmentRoomViewModel()
        {
            _service = new TreatmentRoomService();

            RoomTypeList = TreatmentRoomType.GetRoomTypeList();
            AvailabilityList = TreatmentRoomType.GetAvailabilityList();

            LoadTreatmentRooms();

            AddTreatmentRoomCommand = new RelayCommand(_ => AddTreatmentRoom(), _ => CanAddTreatmentRoom());
            UpdateTreatmentRoomCommand = new RelayCommand(_ => UpdateTreatmentRoom(), _ => CanUpdateTreatmentRoom());
            DeleteTreatmentRoomCommand = new RelayCommand(_ => DeleteTreatmentRoom(), _ => CanDeleteTreatmentRoom());

        }

        private void LoadTreatmentRooms()
        {
            TreatmentRooms.Clear();
            var roomsFromDb = _service.GetAllTreatmentRooms();

            foreach (var room in roomsFromDb)
            {
                TreatmentRooms.Add(room);
            }
        }

        private void AddTreatmentRoom()
        {
            try
            {
                var newRoom = new TreatmentRoom(Name, RoomType, Availability);
                _service.AddTreatmentRoom(newRoom);

                // Odśwież listę z bazy
                LoadTreatmentRooms();

                MessageBox.Show("Dodano salę zabiegową!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            ClearForm();
        }

        private bool CanAddTreatmentRoom()
        {
            return SelectedTreatmentRoom == null &&
                   !string.IsNullOrWhiteSpace(Name) &&
                   !string.IsNullOrWhiteSpace(RoomType) &&
                   !string.IsNullOrWhiteSpace(Availability);
        }


        private void UpdateTreatmentRoom()
        {
            if (SelectedTreatmentRoom == null)
                return;

            try
            {
                SelectedTreatmentRoom.Name = Name;
                SelectedTreatmentRoom.RoomType = RoomType;
                SelectedTreatmentRoom.Availability = Availability;

                _service.UpdateTreatmentRoom(SelectedTreatmentRoom);

                // Odśwież listę z bazy
                LoadTreatmentRooms();

                MessageBox.Show("Zmiany zostały zapisane pomyślnie!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas zapisywania zmian: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanUpdateTreatmentRoom()
        {
            return SelectedTreatmentRoom != null &&
                   !string.IsNullOrWhiteSpace(Name) &&
                   !string.IsNullOrWhiteSpace(RoomType) &&
                   !string.IsNullOrWhiteSpace(Availability);

        }
        private void DeleteTreatmentRoom()
        {
            if (SelectedTreatmentRoom == null)
                return;

            if (_service.HasAppointmentForRoom(SelectedTreatmentRoom.Id))
            {
                MessageBox.Show("Nie można usunąć sali, ponieważ istnieją powiązane wizyty.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                _service.DeleteTreatmentRoom(SelectedTreatmentRoom.Id);
                LoadTreatmentRooms();
                ClearForm();
                MessageBox.Show("Sala została usunięta.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas usuwania: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private bool CanDeleteTreatmentRoom()
        {
            return SelectedTreatmentRoom != null;
        }


        private void ClearForm()
        {
            Name = string.Empty;
            RoomType = null;
            Availability = null;

        }
    }
}
