using FrontEndHealthPets.ViewModels;

namespace FrontEndHealthPets.Paginas;

public partial class WeightTrackingPage : ContentPage
{
    public WeightTrackingPage(WeightTrackingViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        // Set maximum date to today
        DatePickerWeight.MaximumDate = DateTime.Today;
    }
}
