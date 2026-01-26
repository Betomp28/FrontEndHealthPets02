using FrontEndHealthPets.ViewModels;

namespace FrontEndHealthPets.Paginas
{
    public partial class VeterinaryDirectoryPage : ContentPage
    {
        private readonly VeterinaryDirectoryViewModel _viewModel;

        public VeterinaryDirectoryPage(VeterinaryDirectoryViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Fade-in animation
            Content.Opacity = 0;

            if (_viewModel.LoadDataCommand.CanExecute(null))
            {
                _viewModel.LoadDataCommand.Execute(null);
            }

            await Content.FadeTo(1, 400, Easing.CubicOut);
        }
    }
}
