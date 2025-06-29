using SalonKosmetycznyApp.Commands;
using SalonKosmetycznyApp.Model;
using SalonKosmetycznyApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using SalonKosmetycznyApp.Views;
using MySqlX.XDevAPI;
using System.Windows.Navigation;

namespace SalonKosmetycznyApp.ViewModel
{
    internal class EmployeeScheduleViewModel : BaseViewModel
    {
        public ObservableCollection<Employee> Employees { get; } = new ObservableCollection<Employee>();

        private List<Employee> _allEmployees = new List<Employee>();
        public List<TimeSpan> Hours { get; } = new()
{
    new TimeSpan(8, 0, 0),
    new TimeSpan(8, 30, 0),
    new TimeSpan(9, 0, 0),
    new TimeSpan(9, 30, 0),
    new TimeSpan(10, 0, 0),
    new TimeSpan(10, 30, 0),
    new TimeSpan(11, 0, 0),
    new TimeSpan(11, 30, 0),
    new TimeSpan(12, 0, 0),
    new TimeSpan(12, 30, 0),
    new TimeSpan(13, 0, 0),
    new TimeSpan(13, 30, 0),
    new TimeSpan(14, 0, 0),
    new TimeSpan(14, 30, 0),
    new TimeSpan(15, 0, 0),
    new TimeSpan(15, 30, 0),
    new TimeSpan(16, 0, 0),
    new TimeSpan(16, 30, 0),
    new TimeSpan(17, 0, 0),
    new TimeSpan(17, 30, 0),
    new TimeSpan(18, 0, 0)
};


        public EmployeeScheduleViewModel()
            {
                LoadData();
            }
            private readonly ScheduleService _scheduleService = new ScheduleService();
        private readonly EmployeeService _employeeService = new EmployeeService();

        public ObservableCollection<Schedule> WorkSchedule { get; } = new ObservableCollection<Schedule>();

            private ICollectionView _scheduleView;
        public ICollectionView ScheduleView
        {
                get => _scheduleView;
                private set
                {
                _scheduleView = value;
                    OnPropertyChanged(nameof(ScheduleView));
                }
            }
        public void InitializeScheduleView()
        {
            ScheduleView = CollectionViewSource.GetDefaultView(WorkSchedule);
            ScheduleView.Filter = FilterSchedule;
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
                    _scheduleView?.Refresh();
                }
            }
        }
        private Employee _selectedEmployee;
        public Employee SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                if (_selectedEmployee != value)
                {
                    _selectedEmployee = value;
                    EmployeeName = $"{value?.FirstName} {value?.LastName}"; // dla ewentualnego wyświetlania
                    OnPropertyChanged(nameof(SelectedEmployee));
                }
            }
        }
        private bool FilterSchedule(object obj)
        {
            if (obj is Schedule schedule)
            {
                if (string.IsNullOrWhiteSpace(SearchTerm)) return true;

                var term = SearchTerm.ToLower();
                return schedule.EmployeeName.ToLower().Contains(term)
                    || schedule.StartDate.ToString("yyyy-MM-dd").ToLower().Contains(term)
                    || schedule.StartDate.ToString("MM-dd-yyyy").ToLower().Contains(term)
                    || schedule.StartDate.ToString("dd-MM-yyyy").ToLower().Contains(term);
            }
            return false;
        }
        private void LoadEmployees()
        {
            Employees.Clear();
            _allEmployees = _employeeService.GetAllEmployees();

            foreach (var employee in _allEmployees)
                Employees.Add(employee);
        }
        public void LoadData()
            {
                WorkSchedule.Clear();
                var scheduleFromDb = _scheduleService.GetAllSchedules();
                foreach (var schedule in scheduleFromDb)
                    WorkSchedule.Add(schedule);

                InitializeScheduleView();
            LoadEmployees();
            }
            public void ClearForm()
            {
            StartTime = EndTime = null;
            StartDate = null;
            SelectedEmployee = null;
            SelectedSchedule = null;

            OnPropertyChanged(nameof(EmployeeName));
            OnPropertyChanged(nameof(StartDate));
            OnPropertyChanged(nameof(StartTime));
            OnPropertyChanged(nameof(EndTime));
            OnPropertyChanged(nameof(SelectedSchedule));
        }




        private string _employeeName;
        public string EmployeeName
        {
            get => _employeeName;
            set
            {
                if (_employeeName != value)
                {
                    _employeeName = value;
                    OnPropertyChanged(nameof(EmployeeName));
                }
            }
        }
        public DateTime? _startDate;
            public DateTime? StartDate
            {
                get => _startDate;
                set
                {
                    if (_startDate != value)
                    {
                        _startDate = value;
                        OnPropertyChanged(nameof(StartDate));
                    }

                }
            }
           
            public TimeSpan? _startTime;
            public TimeSpan? StartTime
            {
                get => _startTime;
            set
            {
                if (_startTime != value)
                {
                    _startTime = value;
                    OnPropertyChanged(nameof(StartTime));
                }

            }
        }
            public TimeSpan? _endTime;
            public TimeSpan? EndTime
            {
                get => _endTime;
            set
            {
                if (_endTime != value)
                {
                    _endTime = value;
                    OnPropertyChanged(nameof(EndTime));
                }

            }
        }
        
            private Schedule _selectedSchedule;
        public Schedule? SelectedSchedule
        {
            get => _selectedSchedule;
            set
            {
                _selectedSchedule = value;
                if (_selectedSchedule != null)
                {
                    EmployeeName = _selectedSchedule.EmployeeName;
                    StartDate = _selectedSchedule.StartDate;
                    StartTime = _selectedSchedule.StartTime;
                    EndTime = _selectedSchedule.EndTime;

                    // TO JEST KLUCZOWE:
                    SelectedEmployee = _allEmployees
                        .FirstOrDefault(e => e.Id == _selectedSchedule.EmployeeId);

                    OnPropertyChanged(nameof(SelectedSchedule));
                }
            }
        }


        private ICommand _addScheduleCommand;
        public ICommand AddScheduleCommand => _addScheduleCommand ??= new RelayCommand(
            o =>
            {
                if (string.IsNullOrWhiteSpace(EmployeeName) ||
                    StartDate == null || StartTime == null ||
                    EndTime == null)
                {
                    // Możesz dodać obsługę błędu albo komunikat do użytkownika tutaj
                    return;
                }


                if (SelectedEmployee == null ||
     StartDate == null || StartTime == null || EndTime == null)
                {
                    return;
                }

                var schedule = new Schedule(
    SelectedEmployee.Id,
    $"{SelectedEmployee.FirstName} {SelectedEmployee.LastName}",
    StartDate.Value,
    StartTime.Value,
    EndTime.Value);
                _scheduleService.AddSchedule(schedule);

                LoadData();
                ClearForm();
                OnPropertyChanged(nameof(AddScheduleCommand));
            },
            o => {
                return StartDate != null & EmployeeName!=null & StartTime!=null & EndTime!=null & StartTime < EndTime;
            }
        );

        private ICommand _updateScheduleCommand;
        public ICommand UpdateScheduleCommand => _updateScheduleCommand ??= new RelayCommand(
            o =>
            {
                if (_selectedSchedule != null)
                {
                    _selectedSchedule.EmployeeName = EmployeeName;
                    _selectedSchedule.StartDate = (DateTime)StartDate;
                    _selectedSchedule.StartTime = (TimeSpan)StartTime;
                    _selectedSchedule.EndTime = (TimeSpan)EndTime;

                    _scheduleService.UpdateSchedule(_selectedSchedule);
                    LoadData();
                    ClearForm();
                }
            },
            o => {
                return StartDate != null & EmployeeName != null & StartTime != null & EndTime != null & StartTime < EndTime && _selectedSchedule!=null;
            }
        );

        private ICommand _deleteScheduleCCommand;
        public ICommand DeleteScheduleCommand => _deleteScheduleCCommand ??= new RelayCommand(
            o =>
            {
                if (_selectedSchedule != null)
                {
                    _scheduleService.DeleteSchedule(_selectedSchedule.Id);
                    WorkSchedule.Remove(_selectedSchedule);
                    LoadData();
                    ClearForm();
                }
            },
            o => _selectedSchedule != null
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

