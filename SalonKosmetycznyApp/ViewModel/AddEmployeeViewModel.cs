using MySqlX.XDevAPI;
using SalonKosmetycznyApp.Commands;
using SalonKosmetycznyApp.Model;
using SalonKosmetycznyApp.Services;
using SalonKosmetycznyApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace SalonKosmetycznyApp.ViewModel
{
    internal class AddEmployeeViewModel : BaseViewModel
    {
        private readonly EmployeeService _employeeService = new EmployeeService();

        public AddEmployeeViewModel()
        {
            LoadData();
        }

        public ObservableCollection<Employee> Employees { get; } = new ObservableCollection<Employee>();

        public ObservableCollection<string> StatusList { get; } = new()
        {
            "Aktywny",
            "Nieaktywny",
        };
        public void InitializeEmployeesView()
        {
            EmployeeView= CollectionViewSource.GetDefaultView(Employees);
            EmployeeView.Filter = FilterEmployees;
        }
        private ICollectionView _emplyeeView;
        public ICollectionView EmployeeView
        {
            get => _emplyeeView;
            private set
            {
                _emplyeeView = value;
                OnPropertyChanged(nameof(EmployeeView));
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
                    _emplyeeView?.Refresh();
                }
            }
        }
        private bool FilterEmployees(object obj)
        {
            if (obj is Employee employee)
            {
                if (string.IsNullOrWhiteSpace(SearchTerm)) return true;

                var term = SearchTerm.ToLower();
                return employee.FirstName.ToLower().Contains(term)
                    || employee.LastName.ToLower().Contains(term)
                    || (employee.Status?.ToLower().Contains(term) ?? false)
                    || employee.Phone.ToLower().Contains(term)
                    || employee.Position.ToLower().Contains(term)
                    || employee.Email.ToLower().Contains(term);
            }
            return false;
        }
        private void LoadData()
        {
            Employees.Clear();
            var employeesFromDb = _employeeService.GetAllEmployees();
            foreach (var employee in employeesFromDb)
                Employees.Add(employee);
            InitializeEmployeesView();
        }

        public void ClearForm()
        {
            Login = string.Empty;
            Password = string.Empty; 
            Phone = string.Empty;
            Email = string.Empty;
            HireDate = null;
            Position = string.Empty;
            Status = "Aktywny";
            FirstName = string.Empty;
            LastName = string.Empty;
            SelectedEmployee = null;

            var addEmployeeView = Application.Current.Windows
                .OfType<Window>()
                .SelectMany(w => w.FindVisualChildren<AddEmployeeView>())
                .FirstOrDefault();
            addEmployeeView?.ClearPasswordBox();
        }



        private string _login;
        public string Login
        {
            get => _login;
            set
            {
                if (_login != value)
                {
                    _login = value;
                    OnPropertyChanged(nameof(Login));
                }
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged(nameof(Password));

                }
            }
        }

        private string _phone;
        public string Phone
        {
            get => _phone;
            set
            {
                if (_phone != value)
                {
                    _phone = value;
                    OnPropertyChanged(nameof(Phone));
                }
            }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged(nameof(Email));
                }
            }
        }

        private DateTime? _hireDate;
        public DateTime? HireDate
        {
            get => _hireDate;
            set
            {
                if (_hireDate != value)
                {
                    _hireDate = value;
                    OnPropertyChanged(nameof(HireDate));
                }
            }
        }

        private string _position;
        public string Position
        {
            get => _position;
            set
            {
                if (_position != value)
                {
                    _position = value;
                    OnPropertyChanged(nameof(Position));
                }
            }
        }

        private string _status;
        public string Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        private string _firstName;
        public string FirstName
        {
            get => _firstName;
            set
            {
                if (_firstName != value)
                {
                    _firstName = value;
                    OnPropertyChanged(nameof(FirstName));
                }
            }
        }

        private string _lastName;
        public string LastName
        {
            get => _lastName;
            set
            {
                if (_lastName != value)
                {
                    _lastName = value;
                    OnPropertyChanged(nameof(LastName));
                }
            }
        }

        private Employee _selectedEmployee;
        public Employee SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                _selectedEmployee = value;
                if (_selectedEmployee != null)
                {
                    Login = _selectedEmployee.Login;
                    Password = _selectedEmployee.Password; // Pobranie istniejącego hasła
                    Phone = _selectedEmployee.Phone;
                    Email = _selectedEmployee.Email;
                    HireDate = _selectedEmployee.HireDate;
                    Position = _selectedEmployee.Position;
                    Status = _selectedEmployee.Status ?? "Aktywny";
                    FirstName = _selectedEmployee.FirstName;
                    LastName = _selectedEmployee.LastName;
                    OnPropertyChanged(nameof(SelectedEmployee));
                }
            }
        }

        public void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                Password = passwordBox.Password;
            }
        }

        private ICommand _addEmployeeCommand;
        public ICommand AddEmployeeCommand => _addEmployeeCommand ??= new RelayCommand(
            o =>
            {
                var employee = new Employee(
                    Login,
                    Password,
                    Phone,
                    Email,
                    HireDate,
                    Position,
                    Status,
                    FirstName,
                    LastName
                );
                _employeeService.AddEmployee(employee);
                LoadData();
                ClearForm(); // Resetuje hasło na pusty string
                (Application.Current.Windows
            .OfType<Window>()
            .FirstOrDefault(w => w is SalonKosmetycznyApp.Views.AddEmployeeView))?
            .GetType()
            .GetMethod("ClearPasswordBox")
            ?.Invoke(
                Application.Current.Windows.OfType<Window>()
                .FirstOrDefault(w => w is SalonKosmetycznyApp.Views.AddEmployeeView),
                null
            );
                MessageBox.Show("Pracownik dodany pomyślnie.", "", MessageBoxButton.OK, MessageBoxImage.Information);
            },
            o => !string.IsNullOrWhiteSpace(Login) &&
                 !string.IsNullOrWhiteSpace(Password) &&
                 !string.IsNullOrWhiteSpace(Phone) &&
                 !string.IsNullOrWhiteSpace(Email) &&
                 !string.IsNullOrWhiteSpace(Position) &&
                 !string.IsNullOrWhiteSpace(FirstName) &&
                 !string.IsNullOrWhiteSpace(LastName)
        );

        private ICommand _updateEmployeeCommand;
        public ICommand UpdateEmployeeCommand => _updateEmployeeCommand ??= new RelayCommand(
            o =>
            {
                if (SelectedEmployee != null)
                {
                    SelectedEmployee.Login = Login;
                    SelectedEmployee.Password = Password; 
                    SelectedEmployee.Phone = Phone;
                    SelectedEmployee.Email = Email;
                    SelectedEmployee.HireDate = HireDate;
                    SelectedEmployee.Position = Position;
                    SelectedEmployee.Status = Status;
                    SelectedEmployee.FirstName = FirstName;
                    SelectedEmployee.LastName = LastName;
                    _employeeService.UpdateEmployee(SelectedEmployee);
                    LoadData();
                    ClearForm();
                    MessageBox.Show("Pracownik zaktualizowany pomyślnie.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            },
            o => SelectedEmployee != null &&
                 !string.IsNullOrWhiteSpace(Login) &&
                 !string.IsNullOrWhiteSpace(Password) &&
                 !string.IsNullOrWhiteSpace(Phone) &&
                 !string.IsNullOrWhiteSpace(Email) &&
                 !string.IsNullOrWhiteSpace(Position) &&
                 !string.IsNullOrWhiteSpace(FirstName) &&
                 !string.IsNullOrWhiteSpace(LastName)
        );

        private ICommand _deleteEmployeeCommand;
        public ICommand DeleteEmployeeCommand => _deleteEmployeeCommand ??= new RelayCommand(
            o =>
            {
                if (SelectedEmployee != null)
                {
                    _employeeService.DeleteEmployee(SelectedEmployee.Id);
                    Employees.Remove(SelectedEmployee);
                    LoadData();
                    ClearForm();
                    MessageBox.Show("Pracownik usunięty pomyślnie.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            },
            o => SelectedEmployee != null
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

    public static class VisualTreeHelperExtensions
    {
        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T t)
                    {
                        yield return t;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }


}
