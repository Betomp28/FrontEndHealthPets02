using FrontEndHealthPets.Helpers;

namespace FrontEndHealthPets.Pages
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            LoadSettings();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadSettings();
        }

        private void LoadSettings()
        {
            // Load user info
            UserName.Text = Settings.UserName;
            UserEmail.Text = Settings.UserEmail;
            


            // Load notification settings
            NotificationsSwitch.IsToggled = Settings.NotificationsEnabled;
            VaccineRemindersSwitch.IsToggled = Settings.VaccineRemindersEnabled;
            AppointmentRemindersSwitch.IsToggled = Settings.AppointmentRemindersEnabled;

            // App version
            AppVersion.Text = $"Versión {Constants.AppVersion}";
        }

        private async void EditProfile_Clicked(object sender, EventArgs e)
        {
            // TODO: Navigate to edit profile page
            await DisplayAlert("Editar Perfil", "Editar información de perfil próximamente", "OK");
        }

        private async void ChangePassword_Tapped(object sender, TappedEventArgs e)
        {
            // TODO: Navigate to change password page
            await DisplayAlert("Cambiar Contraseña", "Cambio de contraseña próximamente", "OK");
        }

        private async void EmailPreferences_Tapped(object sender, TappedEventArgs e)
        {
            // TODO: Navigate to email preferences page
            await DisplayAlert("Preferencias de Email", "Configuración de email próximamente", "OK");
        }

        private void Notifications_Toggled(object sender, ToggledEventArgs e)
        {
            Settings.NotificationsEnabled = e.Value;
            
            // Disable sub-options if notifications are off
            if (!e.Value)
            {
                VaccineRemindersSwitch.IsToggled = false;
                AppointmentRemindersSwitch.IsToggled = false;
            }
        }

        private void VaccineReminders_Toggled(object sender, ToggledEventArgs e)
        {
            Settings.VaccineRemindersEnabled = e.Value;
        }

        private void AppointmentReminders_Toggled(object sender, ToggledEventArgs e)
        {
            Settings.AppointmentRemindersEnabled = e.Value;
        }



        private async void Language_Tapped(object sender, TappedEventArgs e)
        {
            string language = await DisplayActionSheet(
                "Seleccionar Idioma",
                "Cancelar",
                null,
                "Español",
                "English",
                "Português"
            );

            if (!string.IsNullOrEmpty(language) && language != "Cancelar")
            {
                Settings.Language = language;
                await DisplayAlert("Idioma", $"Idioma cambiado a {language}", "OK");
            }
        }

        private async void PrivacyPolicy_Tapped(object sender, TappedEventArgs e)
        {
            // TODO: Open privacy policy page or URL
            await DisplayAlert("Política de Privacidad", "Próximamente", "OK");
        }

        private async void Terms_Tapped(object sender, TappedEventArgs e)
        {
            // TODO: Open terms of service page or URL
            await DisplayAlert("Términos de Servicio", "Próximamente", "OK");
        }

        private async void About_Tapped(object sender, TappedEventArgs e)
        {
            await DisplayAlert(
                "Acerca de HealthPets",
                $"HealthPets {Constants.AppVersion}\n\n" +
                "Tu compañero perfecto para el cuidado de tus mascotas.\n\n" +
                "© 2024 HealthPets. Todos los derechos reservados.",
                "OK"
            );
        }

        private async void Logout_Clicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert(
                "Cerrar Sesión",
                "¿Estás seguro de que deseas cerrar sesión?",
                "Sí, cerrar sesión",
                "Cancelar"
            );

            if (confirm)
            {
                // Clear all settings
                Settings.ClearAll();

                // Navigate back to login
                Application.Current.MainPage = new NavigationPage(new MainPage());
            }
        }
    }
}
