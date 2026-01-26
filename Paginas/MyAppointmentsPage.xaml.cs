using FrontEndHealthPets.ViewModels;

namespace FrontEndHealthPets.Paginas
{
    public partial class MyAppointmentsPage : ContentPage
    {
        public MyAppointmentsPage(MyAppointmentsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
