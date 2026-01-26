using FrontEndHealthPets.ViewModels;

namespace FrontEndHealthPets.Paginas;

public partial class ForgotPasswordPage : ContentPage
{
	public ForgotPasswordPage(ForgotPasswordViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
