using FrontEndHealthPets.ViewModels;

namespace FrontEndHealthPets.Paginas;

public partial class ClinicsPage : ContentPage
{
    public ClinicsPage(ClinicsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ClinicsViewModel vm)
        {
            await vm.LoadClinicsCommand.ExecuteAsync(null);
        }
    }
}
