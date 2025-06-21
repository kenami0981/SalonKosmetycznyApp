using SalonKosmetycznyApp.Commands;
using SalonKosmetycznyApp.Model;
using SalonKosmetycznyApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SalonKosmetycznyApp.ViewModel
{
    internal class ProductsViewModel : BaseViewModel
    {
        private readonly ProductService _productService = new ProductService();

        public ProductsViewModel()
        {
            LoadData();
        }

        public ObservableCollection<Product> Products { get; } = new ObservableCollection<Product>();

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

        private decimal _price;
        public decimal Price
        {
            get => _price;
            set { _price = value; OnPropertyChanged(nameof(Price)); }
        }

        private int _stock;
        public int ProductStock
        {
            get => _stock;
            set { _stock = value; OnPropertyChanged(nameof(ProductStock)); }
        }

        private Product _selectedProduct;
        public Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                if (value != null)
                {
                    Name = value.Name;
                    Description = value.Description;
                    Price = value.Price;
                    ProductStock = value.ProductStock;
                }
                OnPropertyChanged(nameof(SelectedProduct));
            }
        }

        private ICommand _addProductCommand;
        public ICommand AddProductCommand => _addProductCommand ??= new RelayCommand(
            o =>
            {
                var product = new Product(Name, Description, Price, ProductStock);
                _productService.AddProduct(product);
                LoadData();
                ClearForm();
            },
            o => !string.IsNullOrWhiteSpace(Name) && Price >= 0 && ProductStock >= 0
        );

        public void LoadData()
        {
            Products.Clear();
            var fromDb = _productService.GetAllProducts();
            foreach (var p in fromDb)
                Products.Add(p);

            CheckLowStock();
        }

        public void ClearForm()
        {
            Name = Description = string.Empty;
            Price = 0;
            ProductStock = 0;
            SelectedProduct = null;
        }

        private ICommand _updateProductCommand;
        public ICommand UpdateProductCommand => _updateProductCommand ??= new RelayCommand(
            o =>
            {
                if (SelectedProduct != null)
                {
                    SelectedProduct.Name = Name;
                    SelectedProduct.Description = Description;
                    SelectedProduct.Price = Price;
                    SelectedProduct.ProductStock = ProductStock;

                    _productService.UpdateProduct(SelectedProduct);
                    LoadData();
                    ClearForm();
                }
            },
            o => SelectedProduct != null && !string.IsNullOrWhiteSpace(Name) && Price >= 0 && ProductStock >= 0
        );

        private ICommand _deleteProductCommand;
        public ICommand DeleteProductCommand => _deleteProductCommand ??= new RelayCommand(
            o =>
            {
                if (SelectedProduct != null)
                {
                    _productService.DeleteProduct(SelectedProduct.Id);
                    LoadData();
                    ClearForm();
                }
            },
            o => SelectedProduct != null
        );

        public event Action<int> LowStockDetected;

        private const int StockLimit = 3;

        public void CheckLowStock()
        {
            int lowStockCount = Products.Count(p => p.ProductStock <= StockLimit);
            if (lowStockCount > 0)
            {
                LowStockDetected?.Invoke(lowStockCount);
            }
        }


    }
}
