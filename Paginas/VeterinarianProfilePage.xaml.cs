using FrontEndHealthPets.ViewModels;

namespace FrontEndHealthPets.Paginas
{
    public partial class VeterinarianProfilePage : ContentPage
    {
        public VeterinarianProfilePage(VeterinarianProfileViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
