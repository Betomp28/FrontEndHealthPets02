using FrontEndHealthPets.ViewModels;

namespace FrontEndHealthPets.Paginas
{
    public partial class VeterinariansPage : ContentPage
    {
        public VeterinariansPage(VeterinariansViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
