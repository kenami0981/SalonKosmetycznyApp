using SalonKosmetycznyApp.Model;
using SalonKosmetycznyApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Logika interakcji dla klasy AddTreatmentView.xaml
    /// </summary>
    public partial class AddTreatmentView : UserControl
    {
        public AddTreatmentView()
        {
            InitializeComponent();

        }

        private void ProductsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is AddTreatmentViewModel vm)
            {
                var listBox = sender as ListBox;
                var selectedItems = listBox.SelectedItems.Cast<Product>().ToList();
                vm.SelectedProducts = new ObservableCollection<Product>(selectedItems);
            }
        }

        private void TreatmentsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is AddTreatmentViewModel vm)
            {
                vm.ClearForm();
            }
        }

    }
}
