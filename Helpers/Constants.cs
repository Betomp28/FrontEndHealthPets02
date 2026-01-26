namespace FrontEndHealthPets.Helpers
{
    /// <summary>
    /// Application-wide constants
    /// </summary>
    public static class Constants
    {
        // App Info
        public const string AppName = "HealthPets";
        public const string AppVersion = "1.0.0";

        // API Configuration - Dynamic based on platform
        // Using HTTP for development to avoid SSL certificate issues
        public static string ApiBaseUrl
        {
            get
            {
#if WINDOWS
                return "http://localhost:5152/api/";
#elif ANDROID
                return "http://10.0.2.2:5152/api/";
#else
                return "http://localhost:5152/api/";
#endif
            }
        }

        // SignalR Hub URL for Video Calls
        public static string VideoCallHubUrl
        {
            get
            {
#if WINDOWS
                return "http://localhost:5152/hubs/videocall";
#elif ANDROID
                return "http://10.0.2.2:5152/hubs/videocall";
#else
                return "http://localhost:5152/hubs/videocall";
#endif
            }
        }

        public const int ApiTimeoutSeconds = 30;

        // Freemium Limits
        public const int FreeTierPetLimit = 2;
        public const int FreeTierPhotoLimit = 5; // Photos per pet
        public const bool FreeTierPdfExport = false;
        public const bool FreeTierCloudBackup = false;

        // Premium Pricing
        public const decimal MonthlyPrice = 4.99m;
        public const decimal YearlyPrice = 49.99m;
        public const string Currency = "USD";

        // Date/Time Formats
        public const string DateFormat = "dd/MM/yyyy";
        public const string DateTimeFormat = "dd/MM/yyyy HH:mm";
        public const string TimeFormat = "HH:mm";

        // Error Messages
        public const string NetworkErrorMessage = "No se pudo conectar con el servidor. Verifica tu conexión a internet.";
        public const string ServerErrorMessage = "Ocurrió un error en el servidor. Intenta nuevamente más tarde.";
        public const string GenericErrorMessage = "Ocurrió un error inesperado. Por favor intenta nuevamente.";
        public const string UnauthorizedErrorMessage = "Tu sesión ha expirado. Por favor, inicia sesión nuevamente.";
        public const string ValidationErrorMessage = "Por favor, verifica los datos ingresados.";

        // Validation Rules
        public const int MinPasswordLength = 6;
        public const int MaxPasswordLength = 100;
        public const int MaxPetNameLength = 50;
        public const int MaxNotesLength = 500;

        // File Upload
        public const int MaxPhotoSizeMB = 5;
        public static readonly string[] AllowedImageFormats = { ".jpg", ".jpeg", ".png", ".webp" };

        // Notification IDs Range
        public const int VaccineNotificationRangeStart = 1000;
        public const int AppointmentNotificationRangeStart = 2000;
        public const int MedicationNotificationRangeStart = 3000;

        // Cache Duration (in hours)
        public const int CacheDurationHours = 24;

        // Analytics Events
        public const string EventPetAdded = "pet_added";
        public const string EventVaccineAdded = "vaccine_added";
        public const string EventAppointmentCreated = "appointment_created";
        public const string EventPremiumUpgrade = "premium_upgrade";
    }
}
