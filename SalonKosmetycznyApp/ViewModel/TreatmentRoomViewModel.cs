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
    public class TreatmentRoomViewModel : INotifyPropertyChanged
    {
        private readonly TreatmentRoomService _treatmentRoomService;

        public TreatmentRoomViewModel()
        {
            _treatmentRoomService = new TreatmentRoomService();
            TreatmentRooms = new ObservableCollection<TreatmentRoom>(_treatmentRoomService.GetAllTreatmentRooms());
            AvailabilityList = new ObservableCollection<string> { "Dostępna", "Niedostępna" };

            AddTreatmentRoomCommand = new RelayCommand(_ => AddTreatmentRoom(), _ => true);
            UpdateTreatmentRoomCommand = new RelayCommand(_ => UpdateTreatmentRoom(), _ => SelectedTreatmentRoom != null);
            DeleteTreatmentRoomCommand = new RelayCommand(_ => DeleteTreatmentRoom(), _ => SelectedTreatmentRoom != null);
            AddTreatmentRoomCommand = new RelayCommand(AddTreatmentRoom);
            UpdateTreatmentRoomCommand = new RelayCommand(UpdateTreatmentRoom, () => SelectedTreatmentRoom != null);
            DeleteTreatmentRoomCommand = new RelayCommand(DeleteTreatmentRoom, () => SelectedTreatmentRoom != null);
        }

        // Właściwości do powiązania z formularzem
        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        private string _type;
        public string Type
        {
            get => _type;
            set { _type = value; OnPropertyChanged(); }
        }

        private string _availability;
        public string Availability
        {
            get => _availability;
            set { _availability = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> AvailabilityList { get; }

        // Lista i zaznaczony element
        public ObservableCollection<TreatmentRoom> TreatmentRooms { get; }

        private TreatmentRoom _selectedTreatmentRoom;
        public TreatmentRoom SelectedTreatmentRoom
        {
            get => _selectedTreatmentRoom;
            set
            {
                _selectedTreatmentRoom = value;
                OnPropertyChanged();
                if (value != null)
                {
                    Name = value.Name;
                    Type = value.Type;
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
            var newRoom = new TreatmentRoom
            {
                Name = this.Name,
                Type = this.Type,
                Availability = this.Availability
            };

            _treatmentRoomService.AddTreatmentRoom(newRoom);
            TreatmentRooms.Add(newRoom);

            ClearForm();
        }

        private void UpdateTreatmentRoom()
        {
            if (SelectedTreatmentRoom == null) return;

            SelectedTreatmentRoom.Name = this.Name;
            SelectedTreatmentRoom.Type = this.Type;
            SelectedTreatmentRoom.Availability = this.Availability;

            _treatmentRoomService.UpdateTreatmentRoom(SelectedTreatmentRoom);

            // Odśwież listę
            var index = TreatmentRooms.IndexOf(SelectedTreatmentRoom);
            TreatmentRooms[index] = SelectedTreatmentRoom;

            ClearForm();
        }

        private void DeleteTreatmentRoom()
        {
            if (SelectedTreatmentRoom == null) return;

            _treatmentRoomService.DeleteTreatmentRoom(SelectedTreatmentRoom.Id);
            TreatmentRooms.Remove(SelectedTreatmentRoom);

            ClearForm();
        }

        private void ClearForm()
        {
            Name = string.Empty;
            Type = string.Empty;
            Availability = string.Empty;
            SelectedTreatmentRoom = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

