using SalonKosmetycznyApp.Commands;
using SalonKosmetycznyApp.Model;
using SalonKosmetycznyApp.Services;
using SalonKosmetycznyApp.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
namespace SalonKosmetycznyApp.ViewModel
{
    internal class TreatmentRoomViewModel : BaseViewModel
    {
        private string _name;
        private string _roomType;
        private string _availability;
        private readonly TreatmentRoomService _service;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string RoomType
        {
            get => _roomType;
            set
            {
                _roomType = value;
                OnPropertyChanged(nameof(RoomType));
            }
        }

        public string Availability
        {
            get => _availability;
            set
            {
                _availability = value;
                OnPropertyChanged(nameof(Availability));
            }
        }

        public ObservableCollection<string> RoomTypeList { get; }
        public ObservableCollection<string> AvailabilityList { get; }

        private ObservableCollection<TreatmentRoom> _treatmentRooms;
        public ObservableCollection<TreatmentRoom> TreatmentRooms
        {
            get => _treatmentRooms;
            set
            {
                _treatmentRooms = value;
                OnPropertyChanged(nameof(TreatmentRooms));
            }
        }


        public ICommand AddTreatmentRoomCommand { get; }
        public ICommand UpdateTreatmentRoomCommand { get; }

        private TreatmentRoom _selectedRoom;
        public TreatmentRoom SelectedRoom
        {
            get => _selectedRoom;
            set
            {
                _selectedRoom = value;
                OnPropertyChanged(nameof(SelectedRoom));

                if (_selectedRoom != null)
                {
                    Name = _selectedRoom.Name;
                    RoomType = _selectedRoom.RoomType;
                    Availability = _selectedRoom.Availability;
                }
            }
        }

        public TreatmentRoomViewModel()
        {
            _service = new TreatmentRoomService();
            RoomTypeList = TreatmentRoomType.GetRoomTypeList();
            AvailabilityList = TreatmentRoomType.GetAvailabilityList();
            TreatmentRooms = new ObservableCollection<TreatmentRoom>(_service.GetAllTreatmentRooms());
            AddTreatmentRoomCommand = new RelayCommand(param => AddTreatmentRoom(), param => true);
            UpdateTreatmentRoomCommand = new RelayCommand(param => UpdateTreatmentRoom(), param => true);
        }

        private void AddTreatmentRoom()
        {
            try
            {
                var newRoom = new TreatmentRoom(Name, RoomType, Availability);
                _service.AddTreatmentRoom(newRoom);
                TreatmentRooms.Add(newRoom);
                MessageBox.Show("Dodano salę zabiegową!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd: {ex.Message}");
            }
        }


        private void UpdateTreatmentRoom()
        {
            var updatedRoom = new TreatmentRoom(Name, RoomType, Availability) { Id = SelectedRoom?.Id ?? 0 };
            _service.UpdateTreatmentRoom(updatedRoom);
        }
    }
}
