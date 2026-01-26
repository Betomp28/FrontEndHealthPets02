using FrontEndHealthPets.ViewModels;

namespace FrontEndHealthPets.Paginas
{
    public partial class VetProfileEditPage : ContentPage
    {
        public VetProfileEditPage(VetProfileEditViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
