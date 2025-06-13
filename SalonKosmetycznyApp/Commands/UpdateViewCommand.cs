using SalonKosmetycznyApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Text;
using System.Threading.Tasks;

namespace SalonKosmetycznyApp.Commands
{
    internal class UpdateViewCommand : ICommand
    {
        private MainViewModel viewModel;

        public UpdateViewCommand(MainViewModel viewModel)
        {
            this.viewModel = viewModel;
        }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (parameter.ToString() == "client")
            {
                viewModel.SelectedViewModel = new AddClientViewModel();
            }
            else if (parameter.ToString() == "employee")
            {
                viewModel.SelectedViewModel = new AddEmployeeViewModel();
            }
            else if (parameter.ToString() == "treatment")
            {
                viewModel.SelectedViewModel = new AddTreatmentViewModel();
            }
            else if (parameter.ToString() == "calendar")
            {
                viewModel.SelectedViewModel = new CalendarViewModel();
            }
            else if (parameter.ToString() == "room")
            {
                viewModel.SelectedViewModel = new AddRoomViewModel();
            }
            else if (parameter.ToString() == "booking")
            {
                viewModel.SelectedViewModel = new AppointmentBookingViewModel();
            }
            else if (parameter.ToString() == "products")
            {
                viewModel.SelectedViewModel = new ProductsViewModel();
            }


        }
    }
}
