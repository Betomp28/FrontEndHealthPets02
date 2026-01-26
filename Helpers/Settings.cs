namespace FrontEndHealthPets.Helpers
{
    /// <summary>
    /// Manages application settings using Preferences
    /// </summary>
    public static class Settings
    {
        // Authentication
        public static string AuthToken
        {
            get => Preferences.Get(nameof(AuthToken), string.Empty);
            set => Preferences.Set(nameof(AuthToken), value);
        }

        public static string RefreshToken
        {
            get => Preferences.Get(nameof(RefreshToken), string.Empty);
            set => Preferences.Set(nameof(RefreshToken), value);
        }

        public static long UserId
        {
            get => Preferences.Get(nameof(UserId), 0L);
            set => Preferences.Set(nameof(UserId), value);
        }

        public static string UserName
        {
            get => Preferences.Get(nameof(UserName), "Usuario");
            set => Preferences.Set(nameof(UserName), value);
        }

        public static string UserEmail
        {
            get => Preferences.Get(nameof(UserEmail), string.Empty);
            set => Preferences.Set(nameof(UserEmail), value);
        }





        // Remember Me
        public static bool RememberMe
        {
            get => Preferences.Get(nameof(RememberMe), false);
            set => Preferences.Set(nameof(RememberMe), value);
        }

        // Notification Settings
        public static bool NotificationsEnabled
        {
            get => Preferences.Get(nameof(NotificationsEnabled), true);
            set => Preferences.Set(nameof(NotificationsEnabled), value);
        }

        public static bool VaccineRemindersEnabled
        {
            get => Preferences.Get(nameof(VaccineRemindersEnabled), true);
            set => Preferences.Set(nameof(VaccineRemindersEnabled), value);
        }

        public static bool AppointmentRemindersEnabled
        {
            get => Preferences.Get(nameof(AppointmentRemindersEnabled), true);
            set => Preferences.Set(nameof(AppointmentRemindersEnabled), value);
        }

        public static bool MedicationRemindersEnabled
        {
            get => Preferences.Get(nameof(MedicationRemindersEnabled), true);
            set => Preferences.Set(nameof(MedicationRemindersEnabled), value);
        }

        // App Settings
        public static string Language
        {
            get => Preferences.Get(nameof(Language), "Español");
            set => Preferences.Set(nameof(Language), value);
        }

        public static bool DarkModeEnabled
        {
            get => Preferences.Get(nameof(DarkModeEnabled), false);
            set => Preferences.Set(nameof(DarkModeEnabled), value);
        }

        // First Launch
        public static bool IsFirstLaunch
        {
            get => Preferences.Get(nameof(IsFirstLaunch), true);
            set => Preferences.Set(nameof(IsFirstLaunch), value);
        }

        public static DateTime LastSyncDate
        {
            get
            {
                var ticks = Preferences.Get(nameof(LastSyncDate), 0L);
                return ticks > 0 ? new DateTime(ticks) : DateTime.MinValue;
            }
            set => Preferences.Set(nameof(LastSyncDate), value.Ticks);
        }

        // Clear all settings
        public static void ClearAll()
        {
            Preferences.Clear();
        }

        /// <summary>
        /// Clear authentication data
        /// </summary>
        public static void ClearAuth()
        {
            AuthToken = string.Empty;
            RefreshToken = string.Empty;
            UserId = 0;
            UserEmail = string.Empty;
            UserName = string.Empty;
        }

        /// <summary>
        /// Check if user is authenticated
        /// </summary>
        public static bool IsAuthenticated => !string.IsNullOrEmpty(AuthToken);

        // ============ Veterinarian Portal Settings ============
        
        /// <summary>
        /// Vet authentication token
        /// </summary>
        public static string VetAuthToken
        {
            get => Preferences.Get(nameof(VetAuthToken), string.Empty);
            set => Preferences.Set(nameof(VetAuthToken), value);
        }

        public static int VetId
        {
            get => Preferences.Get(nameof(VetId), 0);
            set => Preferences.Set(nameof(VetId), value);
        }

        public static string VetName
        {
            get => Preferences.Get(nameof(VetName), string.Empty);
            set => Preferences.Set(nameof(VetName), value);
        }

        public static string VetEmail
        {
            get => Preferences.Get(nameof(VetEmail), string.Empty);
            set => Preferences.Set(nameof(VetEmail), value);
        }

        public static string VetSpecialty
        {
            get => Preferences.Get(nameof(VetSpecialty), string.Empty);
            set => Preferences.Set(nameof(VetSpecialty), value);
        }

        /// <summary>
        /// Check if veterinarian is authenticated
        /// </summary>
        public static bool IsVetAuthenticated => !string.IsNullOrEmpty(VetAuthToken);

        /// <summary>
        /// Clear veterinarian authentication data
        /// </summary>
        public static void ClearVetAuth()
        {
            VetAuthToken = string.Empty;
            VetId = 0;
            VetName = string.Empty;
            VetEmail = string.Empty;
            VetSpecialty = string.Empty;
        }
    }
}
