using FrontEndHealthPets.Helpers;
using FrontEndHealthPets.Services;
using System.Windows.Input;

namespace FrontEndHealthPets.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _authService;
        private readonly INavigationService _navigationService;

        private string _userName;
        private string _userEmail;

        private bool _notificationsEnabled;
        private bool _vaccineRemindersEnabled;
        private bool _appointmentRemindersEnabled;
        private string _appVersion;

        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        public string UserEmail
        {
            get => _userEmail;
            set => SetProperty(ref _userEmail, value);
        }



        public bool NotificationsEnabled
        {
            get => _notificationsEnabled;
            set
            {
                if (SetProperty(ref _notificationsEnabled, value))
                {
                    Settings.NotificationsEnabled = value;
                    if (!value)
                    {
                        VaccineRemindersEnabled = false;
                        AppointmentRemindersEnabled = false;
                    }
                }
            }
        }

        public bool VaccineRemindersEnabled
        {
            get => _vaccineRemindersEnabled;
            set
            {
                if (SetProperty(ref _vaccineRemindersEnabled, value))
                {
                    Settings.VaccineRemindersEnabled = value;
                }
            }
        }

        public bool AppointmentRemindersEnabled
        {
            get => _appointmentRemindersEnabled;
            set
            {
                if (SetProperty(ref _appointmentRemindersEnabled, value))
                {
                    Settings.AppointmentRemindersEnabled = value;
                }
            }
        }

        public string AppVersion
        {
            get => _appVersion;
            set => SetProperty(ref _appVersion, value);
        }

        public ICommand EditProfileCommand { get; }
        public ICommand ChangePasswordCommand { get; }

        public ICommand LogoutCommand { get; }

        public SettingsViewModel(IAuthenticationService authService, INavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;

            Title = "Configuración";

            EditProfileCommand = new Command(async () => await OnEditProfile());
            ChangePasswordCommand = new Command(async () => await OnChangePassword());

            LogoutCommand = new Command(async () => await OnLogout());

            LoadSettings();
        }

        private void LoadSettings()
        {
            UserName = Settings.UserName;
            UserEmail = Settings.UserEmail;

            NotificationsEnabled = Settings.NotificationsEnabled;
            VaccineRemindersEnabled = Settings.VaccineRemindersEnabled;
            AppointmentRemindersEnabled = Settings.AppointmentRemindersEnabled;
            AppVersion = $"Versión {Constants.AppVersion}";
        }

        private async Task OnEditProfile()
        {
            await ShowAlert("Editar Perfil", "Edición de perfil próximamente", "OK");
        }

        private async Task OnChangePassword()
        {
            await ShowAlert("Cambiar Contraseña", "Cambio de contraseña próximamente", "OK");
        }



        private async Task OnLogout()
        {
            bool confirm = await ShowConfirmation(
                "Cerrar Sesión",
                "¿Estás seguro de que deseas cerrar sesión?",
                "Sí, cerrar",
                "Cancelar"
            );

            if (confirm)
            {
                Settings.ClearAll();
                await _navigationService.NavigateToAsync("///MainPage");
            }
        }
    }
}
