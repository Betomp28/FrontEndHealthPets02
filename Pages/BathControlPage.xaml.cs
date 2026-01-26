using FrontEndHealthPets.ViewModels;

namespace FrontEndHealthPets.Pages
{
    public partial class BathControlPage : ContentPage
    {
        public BathControlPage()
        {
            InitializeComponent();
            // Assign ViewModel explicitly if BindingContext doesn't auto-wire or if we want access to it
            // BindingContext is set in XAML, but we can verify here
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is BathControlViewModel viewModel)
            {
                await viewModel.InitializeAsync();
            }
        }
    }
}
