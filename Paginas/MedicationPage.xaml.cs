using FrontEndHealthPets.ViewModels;

namespace FrontEndHealthPets.Paginas;

public partial class MedicationPage : ContentPage
{
    public MedicationPage(MedicationViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
