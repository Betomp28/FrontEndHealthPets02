namespace FrontEndHealthPets.Paginas.VetPortal;

public partial class VetDashboardPage : ContentPage
{
    public VetDashboardPage(ViewModels.VetPortal.VetDashboardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ViewModels.VetPortal.VetDashboardViewModel vm)
        {
            vm.LoadDataCommand.Execute(null);
        }
    }
}
