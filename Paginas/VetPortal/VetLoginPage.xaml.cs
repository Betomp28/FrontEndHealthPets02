namespace FrontEndHealthPets.Paginas.VetPortal;

public partial class VetLoginPage : ContentPage
{
    public VetLoginPage(ViewModels.VetPortal.VetLoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
