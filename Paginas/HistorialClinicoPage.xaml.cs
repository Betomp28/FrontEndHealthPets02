using FrontEndHealthPets.ViewModels;

namespace FrontEndHealthPets.Paginas
{
    public partial class HistorialClinicoPage : ContentPage
    {
        public HistorialClinicoPage(HistorialClinicoViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
