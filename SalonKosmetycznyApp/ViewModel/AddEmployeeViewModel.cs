using SalonKosmetycznyApp.Commands;
using SalonKosmetycznyApp.Model;
using SalonKosmetycznyApp.Services;
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

namespace SalonKosmetycznyApp.ViewModel
{
    internal class AddEmployeeViewModel : BaseViewModel
    {

        public AddEmployeeViewModel()
        {
            LoadData();
        }

        #region Attached Property for PasswordBox
        public static readonly DependencyProperty BoundPasswordProperty =
            DependencyProperty.RegisterAttached("BoundPassword",
                typeof(string), typeof(AddEmployeeViewModel),
                new PropertyMetadata(string.Empty, OnBoundPasswordChanged));

        public static string GetBoundPassword(DependencyObject dp)
        {
            return (string)dp.GetValue(BoundPasswordProperty);
        }

        public static void SetBoundPassword(DependencyObject dp, string value)
        {
            dp.SetValue(BoundPasswordProperty, value);
        }

        private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
                if (!string.IsNullOrEmpty(e.NewValue as string))
                {
                    passwordBox.Password = e.NewValue as string;
                }
                passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
            }
        }

        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                SetBoundPassword(passwordBox, passwordBox.Password);
                if (passwordBox.DataContext is AddEmployeeViewModel viewModel)
                {
                    viewModel.Password = passwordBox.Password;
                }
            }
        }
        #endregion

        private readonly EmployeeService _employeeService = new EmployeeService();

        public ObservableCollection<Employee> Employees { get; } = new ObservableCollection<Employee>();

        private ICollectionView _employeesView;
        public ICollectionView EmployeesView
        {
            get => _employeesView;
            private set
            {
                _employeesView = value;
                OnPropertyChanged(nameof(EmployeesView));
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
                    EmployeesView?.Refresh();
                }
            }
        }

        public void LoadData()
        {
            Employees.Clear();
            var employeesFromDb = _employeeService.GetAllEmployees();
            foreach (var employee in employeesFromDb)
                Employees.Add(employee);

            InitializeEmployeesView();
        }

        public void ClearForm()
        {
            Login = Password = Phone = Email = Position = Status = FirstName = LastName = string.Empty;
            HireDate = null;
            SelectedEmployee = null;
        }

        public void InitializeEmployeesView()
        {
            EmployeesView = CollectionViewSource.GetDefaultView(Employees);
            EmployeesView.Filter = FilterEmployees;
        }

        private bool FilterEmployees(object obj)
        {
            if (obj is Employee employee)
            {
                if (string.IsNullOrWhiteSpace(SearchTerm)) return true;

                var term = SearchTerm.ToLower();
                return employee.FirstName.ToLower().Contains(term)
                    || employee.LastName.ToLower().Contains(term)
                    || employee.Position.ToLower().Contains(term)
                    || (employee.Phone?.ToLower().Contains(term) ?? false)
                    || (employee.Email?.ToLower().Contains(term) ?? false);
            }
            return false;
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
                    Password = _selectedEmployee.Password; // Uwaga: W rzeczywistej aplikacji nie pokazuj hasła
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
                ClearForm();
                MessageBox.Show("Employee added successfully.", "", MessageBoxButton.OK, MessageBoxImage.Information);
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
                    SelectedEmployee.Password = Password; // Uwaga: W rzeczywistej aplikacji haszuj hasło
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
                    MessageBox.Show("Employee updated successfully.", "", MessageBoxButton.OK, MessageBoxImage.Information);
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
                    MessageBox.Show("Employee deleted successfully.", "", MessageBoxButton.OK, MessageBoxImage.Information);
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
}