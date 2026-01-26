using FrontEndHealthPets.ViewModels;

namespace FrontEndHealthPets.Paginas
{
    public partial class ResenasPage : ContentPage
    {
        public ResenasPage(ResenasViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
