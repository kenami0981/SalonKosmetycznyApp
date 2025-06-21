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
using System.Windows.Data;
using System.Windows.Input;

namespace SalonKosmetycznyApp.ViewModel
{
    internal class AddTreatmentViewModel : BaseViewModel
    {
        private readonly TreatmentService _treatmentService = new TreatmentService();
        private readonly ProductService _productService = new ProductService();

        public AddTreatmentViewModel()
        {
            LoadData();
        }

        public ObservableCollection<Treatment> Treatments { get; } = new ObservableCollection<Treatment>();
        public ObservableCollection<Product> AllProducts { get; } = new ObservableCollection<Product>();
        private ObservableCollection<Product> _selectedProducts = new ObservableCollection<Product>();
        public ObservableCollection<Product> SelectedProducts
        {
            get => _selectedProducts;
            set
            {
                _selectedProducts = value;
                OnPropertyChanged(nameof(SelectedProducts));
            }
        }

        private ICollectionView _treatmentsView;
        public ICollectionView TreatmentsView
        {
            get => _treatmentsView;
            private set
            {
                _treatmentsView = value;
                OnPropertyChanged(nameof(TreatmentsView));
            }
        }

        private string _searchTerm;
        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                _searchTerm = value;
                OnPropertyChanged(nameof(SearchTerm));
                TreatmentsView?.Refresh();
            }
        }

        public void LoadData()
        {
            Treatments.Clear();
            AllProducts.Clear();

            var fromDb = _treatmentService.GetAllTreatments();
            foreach (var t in fromDb)
                Treatments.Add(t);

            var products = _productService.GetAllProducts();
            foreach (var p in products)
                AllProducts.Add(p);

            InitializeTreatmentsView();
        }

        private void InitializeTreatmentsView()
        {
            TreatmentsView = CollectionViewSource.GetDefaultView(Treatments);
            TreatmentsView.Filter = FilterTreatments;
        }

        private bool FilterTreatments(object obj)
        {
            if (obj is Treatment t)
            {
                if (string.IsNullOrWhiteSpace(SearchTerm)) return true;
                var term = SearchTerm.ToLower();
                return t.Name.ToLower().Contains(term)
                    || t.TreatmentType.ToString().ToLower().Contains(term);
            }
            return false;
        }

        public void ClearForm()
        {
            Name = Description = string.Empty;
            DurationMinutes = 0;
            Price = 0;
            TreatmentType = TreatmentType.Twarz;
            SelectedTreatment = null;
            SelectedProducts.Clear();
        }

        // Właściwości formularza
        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(nameof(Name)); }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(nameof(Description)); }
        }

        private int _durationMinutes;
        public int DurationMinutes
        {
            get => _durationMinutes;
            set { _durationMinutes = value; OnPropertyChanged(nameof(DurationMinutes)); }
        }

        private decimal _price;
        public decimal Price
        {
            get => _price;
            set { _price = value; OnPropertyChanged(nameof(Price)); }
        }

        private TreatmentType _treatmentType;
        public TreatmentType TreatmentType
        {
            get => _treatmentType;
            set { _treatmentType = value; OnPropertyChanged(nameof(TreatmentType)); }
        }

        public Array AvailableTreatmentTypes => Enum.GetValues(typeof(TreatmentType));

        private Treatment _selectedTreatment;
        public Treatment SelectedTreatment
        {
            get => _selectedTreatment;
            set
            {
                _selectedTreatment = value;
                if (value != null)
                {
                    Name = value.Name;
                    Description = value.Description;
                    DurationMinutes = (int)value.Duration.TotalMinutes;
                    Price = value.Price;
                    TreatmentType = value.TreatmentType;

                    SelectedProducts.Clear();
                    foreach (var p in value.Products)
                        SelectedProducts.Add(p);
                }
                OnPropertyChanged(nameof(SelectedTreatment));
            }
        }

        public ICommand AddTreatmentCommand => new RelayCommand(
            o =>
            {
                var treatment = new Treatment(Name, Description, TimeSpan.FromMinutes(DurationMinutes), Price, TreatmentType)
                {
                    Products = SelectedProducts.ToList()
                };
                _treatmentService.AddTreatment(treatment);
                LoadData();
                ClearForm();
            },
            o => !string.IsNullOrWhiteSpace(Name) && DurationMinutes > 0 && Price > 0
        );

        public ICommand UpdateTreatmentCommand => new RelayCommand(
            o =>
            {
                if (_selectedTreatment != null)
                {
                    _selectedTreatment.Name = Name;
                    _selectedTreatment.Description = Description;
                    _selectedTreatment.Duration = TimeSpan.FromMinutes(DurationMinutes);
                    _selectedTreatment.Price = Price;
                    _selectedTreatment.TreatmentType = TreatmentType;
                    _selectedTreatment.Products = SelectedProducts.ToList();

                    _treatmentService.UpdateTreatment(_selectedTreatment);
                    LoadData();
                    ClearForm();
                }
            },
            o => _selectedTreatment != null && !string.IsNullOrWhiteSpace(Name)
        );

        public ICommand DeleteTreatmentCommand => new RelayCommand(
            o =>
            {
                if (_selectedTreatment != null)
                {
                    _treatmentService.DeleteTreatment(_selectedTreatment.Id);
                    LoadData();
                    ClearForm();
                }
            },
            o => _selectedTreatment != null
        );
    }
}   

