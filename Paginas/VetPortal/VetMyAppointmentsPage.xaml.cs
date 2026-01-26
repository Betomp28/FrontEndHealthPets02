namespace FrontEndHealthPets.Paginas.VetPortal;

public partial class VetMyAppointmentsPage : ContentPage
{
    public VetMyAppointmentsPage(ViewModels.VetPortal.VetMyAppointmentsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ViewModels.VetPortal.VetMyAppointmentsViewModel vm)
        {
            vm.LoadDataCommand.Execute(null);
        }
    }
}
