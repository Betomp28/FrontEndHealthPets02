using FrontEndHealthPets.ViewModels;

namespace FrontEndHealthPets.Paginas;

public partial class VaccinationPage : ContentPage
{
	public VaccinationPage(VaccinationViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
	}
}
