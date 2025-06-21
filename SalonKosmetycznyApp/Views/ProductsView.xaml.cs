using SalonKosmetycznyApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SalonKosmetycznyApp.Views
{
    /// <summary>
    /// Logika interakcji dla klasy ProductsView.xaml
    /// </summary>
    public partial class ProductsView : UserControl
    {
        private ProductsViewModel _viewModel;

        public ProductsView()
        {
            InitializeComponent();
            _viewModel = new ProductsViewModel();
            DataContext = _viewModel;

            Loaded += ProductsView_Loaded;
        }

        private void ProductsView_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.LowStockDetected += ShowLowStockWarning;

            Dispatcher.InvokeAsync(() =>
            {
                _viewModel.CheckLowStock();
            }, System.Windows.Threading.DispatcherPriority.ApplicationIdle);
        }


        private void ShowLowStockWarning(int count)
        {
            MessageBox.Show(
                $"Uwaga! Masz {count} produktów poniżej limitu. Uzupełnij zapas.",
                "Niski stan magazynowy",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }


    }
}
