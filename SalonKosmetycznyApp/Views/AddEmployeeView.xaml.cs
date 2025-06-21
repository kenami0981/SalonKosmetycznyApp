using SalonKosmetycznyApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Logika interakcji dla klasy AddEmployeeView.xaml
    /// </summary>
    public partial class AddEmployeeView : UserControl
    {
        public AddEmployeeView()
        {
            InitializeComponent();
            this.DataContext = new AddEmployeeViewModel();
        }

        public void ClearPasswordBox()
        {
            PasswordInput.Password = string.Empty;
        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is AddEmployeeViewModel viewModel && sender is PasswordBox passwordBox)
            {
                viewModel.Password = passwordBox.Password;
            }
        }

        private void PhoneTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Pozwalaj tylko na cyfry
            e.Handled = !Regex.IsMatch(e.Text, @"^[0-9]+$");
        }
    }
}