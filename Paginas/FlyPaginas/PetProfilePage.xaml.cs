using FrontEndHealthPets.ViewModels;

namespace FrontEndHealthPets.Paginas.FlyPaginas
{
    public partial class PetProfilePage : ContentPage
    {
        private readonly PetProfileViewModel _viewModel;

        public PetProfilePage(PetProfileViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            
            // Initial state for animation
            Content.Opacity = 0;
            
            if (_viewModel.LoadDataCommand.CanExecute(null))
            {
                _viewModel.LoadDataCommand.Execute(null);
            }

            // Entry animation
            await Content.FadeTo(1, 500, Easing.CubicOut);
        }
    }
}
