namespace FrontEndHealthPets.Views
{
    /// <summary>
    /// Base ContentPage with ViewModel binding
    /// </summary>
    public abstract class BaseContentPage<TViewModel> : ContentPage where TViewModel : ViewModels.BaseViewModel
    {
        protected TViewModel ViewModel => (TViewModel)BindingContext;

        protected BaseContentPage(TViewModel viewModel)
        {
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            
            if (ViewModel != null)
            {
                await ViewModel.OnAppearing();
            }
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            
            if (ViewModel != null)
            {
                await ViewModel.OnDisappearing();
            }
        }
    }
}
