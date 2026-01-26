using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FrontEndHealthPets.ViewModels
{
    /// <summary>
    /// Base ViewModel implementing INotifyPropertyChanged
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        private bool _isBusy;
        private string _title = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets or sets a value indicating whether the ViewModel is busy
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        /// <summary>
        /// Gets or sets the title
        /// </summary>
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        /// <summary>
        /// Gets a value indicating whether the ViewModel is not busy
        /// </summary>
        public bool IsNotBusy => !IsBusy;

        /// <summary>
        /// Sets the property and raises PropertyChanged event
        /// </summary>
        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action? onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            
            // Update IsNotBusy if IsBusy changed
            if (propertyName == nameof(IsBusy))
                OnPropertyChanged(nameof(IsNotBusy));

            return true;
        }

        /// <summary>
        /// Raises the PropertyChanged event
        /// </summary>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Called when the ViewModel appears
        /// </summary>
        public virtual Task OnAppearing()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called when the ViewModel disappears
        /// </summary>
        public virtual Task OnDisappearing()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Shows an alert dialog
        /// </summary>
        protected async Task ShowAlert(string title, string message, string cancel = "OK")
        {
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert(title, message, cancel);
            }
        }

        /// <summary>
        /// Shows a confirmation dialog
        /// </summary>
        protected async Task<bool> ShowConfirmation(string title, string message, string accept = "Sí", string cancel = "No")
        {
            if (Application.Current?.MainPage != null)
            {
                return await Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);
            }
            return false;
        }

        /// <summary>
        /// Shows a loading indicator
        /// </summary>
        protected async Task ExecuteWithLoading(Func<Task> task, string? loadingMessage = null)
        {
            try
            {
                IsBusy = true;
                await task();
            }
            catch (Exception ex)
            {
                await HandleException(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Handles exceptions
        /// </summary>
        protected virtual async Task HandleException(Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
            
            string message = ex.Message;
            
            if (ex is HttpRequestException)
            {
                message = Helpers.Constants.NetworkErrorMessage;
            }
            else if (ex is UnauthorizedAccessException)
            {
                message = Helpers.Constants.UnauthorizedErrorMessage;
                // TODO: Navigate to login
            }
            else
            {
                message = Helpers.Constants.GenericErrorMessage;
            }

            await ShowAlert("Error", message);
        }
    }
}
