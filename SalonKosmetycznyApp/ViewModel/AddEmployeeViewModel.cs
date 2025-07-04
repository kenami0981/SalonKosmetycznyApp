﻿using MySqlX.XDevAPI;
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
            CheckEmployeeExistence();
            IsLoggedIn = false; // Domyślnie niezalogowany
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

        private bool _hasEmployees;
        public bool HasEmployees
        {
            get => _hasEmployees;
            set
            {
                if (_hasEmployees != value)
                {
                    _hasEmployees = value;
                    OnPropertyChanged(nameof(HasEmployees));
                }
            }
        }

        private bool _isLoggedIn;
        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set
            {
                if (_isLoggedIn != value)
                {
                    _isLoggedIn = value;
                    OnPropertyChanged(nameof(IsLoggedIn));
                }
            }
        }

        private void CheckEmployeeExistence()
        {
            HasEmployees = _employeeService.GetAllEmployees().Any();
        }

        public void ClearForm()
        {
            Login = string.Empty;
            Password = string.Empty;
            Phone = null;
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

        private int? _phone;
        public int? Phone
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
                    Password = _selectedEmployee.Password;
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

        private ICommand _loginCommand;
        public ICommand LoginCommand => _loginCommand ??= new RelayCommand(
            o =>
            {
                if (!HasEmployees)
                {
                    MessageBox.Show("Nie można się zalogować, ponieważ nie ma żadnych pracowników w bazie danych.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var employees = _employeeService.GetAllEmployees();
                var employee = employees.FirstOrDefault(e => e.Login == Login && e.Password == Password);

                if (employee != null)
                {
                    IsLoggedIn = true;
                    MessageBox.Show("Logowanie udane! Witaj, " + employee.FirstName + " " + employee.LastName + "!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearForm(); // Opcjonalne czyszczenie pól po zalogowaniu
                }
                else
                {
                    MessageBox.Show("Nieprawidłowy login lub hasło.", "Błąd logowania", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            },
            o => HasEmployees && !string.IsNullOrWhiteSpace(Login) && !string.IsNullOrWhiteSpace(Password)
        );

        private ICommand _addEmployeeCommand;
        public ICommand AddEmployeeCommand => _addEmployeeCommand ??= new RelayCommand(
            o =>
            {
                if (!IsLoggedIn)
                {
                    MessageBox.Show("Musisz się najpierw zalogować, aby dodać pracownika.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
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
                ClearForm();
                CheckEmployeeExistence();
                MessageBox.Show("Pracownik dodany pomyślnie.", "", MessageBoxButton.OK, MessageBoxImage.Information);
            },
            o => IsLoggedIn && !string.IsNullOrWhiteSpace(Login) &&
                 !string.IsNullOrWhiteSpace(Password) &&
                 Phone>0 &&
                 !string.IsNullOrWhiteSpace(Email) &&
                 !string.IsNullOrWhiteSpace(Position) &&
                 !string.IsNullOrWhiteSpace(FirstName) &&
                 !string.IsNullOrWhiteSpace(LastName)
        );

        private ICommand _updateEmployeeCommand;
        public ICommand UpdateEmployeeCommand => _updateEmployeeCommand ??= new RelayCommand(
            o =>
            {
                if (!IsLoggedIn)
                {
                    MessageBox.Show("Musisz się najpierw zalogować, aby zaktualizować pracownika.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (SelectedEmployee != null)
                {
                    SelectedEmployee.Login = Login;
                    SelectedEmployee.Password = Password; 
                    SelectedEmployee.Phone = (int)Phone;

                    SelectedEmployee.Email = Email;
                    SelectedEmployee.HireDate = HireDate;
                    SelectedEmployee.Position = Position;
                    SelectedEmployee.Status = Status;
                    SelectedEmployee.FirstName = FirstName;
                    SelectedEmployee.LastName = LastName;
                    _employeeService.UpdateEmployee(SelectedEmployee);
                    LoadData();
                    ClearForm();
                    CheckEmployeeExistence();
                    MessageBox.Show("Pracownik zaktualizowany pomyślnie.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            },
            o => IsLoggedIn && SelectedEmployee != null &&
                 !string.IsNullOrWhiteSpace(Login) &&
                 !string.IsNullOrWhiteSpace(Password) &&
                 Phone > 0 &&
                 !string.IsNullOrWhiteSpace(Email) &&
                 !string.IsNullOrWhiteSpace(Position) &&
                 !string.IsNullOrWhiteSpace(FirstName) &&
                 !string.IsNullOrWhiteSpace(LastName)
        );

        private ICommand _deleteEmployeeCommand;
        public ICommand DeleteEmployeeCommand => _deleteEmployeeCommand ??= new RelayCommand(
            o =>
            {
                if (!IsLoggedIn)
                {
                    MessageBox.Show("Musisz się najpierw zalogować, aby usunąć pracownika.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (SelectedEmployee != null)
                {
                    _employeeService.DeleteEmployee(SelectedEmployee.Id);
                    Employees.Remove(SelectedEmployee);
                    LoadData();
                    ClearForm();
                    CheckEmployeeExistence();
                    MessageBox.Show("Pracownik usunięty pomyślnie.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            },
            o => IsLoggedIn && SelectedEmployee != null
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