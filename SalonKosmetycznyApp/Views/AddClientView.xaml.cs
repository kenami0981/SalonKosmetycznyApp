﻿using SalonKosmetycznyApp.ViewModel;
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
    /// Logika interakcji dla klasy AddClientView.xaml
    /// </summary>
    public partial class AddClientView : UserControl
    {
        public AddClientView()
        {
            InitializeComponent();
            DataContext = new AddClientViewModel();
        }
    }
}
