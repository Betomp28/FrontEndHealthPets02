using Microsoft.Extensions.Logging;
using FrontEndHealthPets.Services;
using FrontEndHealthPets.ViewModels;
using FrontEndHealthPets.Pages;
using Microcharts.Maui;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace FrontEndHealthPets
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseSkiaSharp() // Enable SkiaSharp for charts
                .UseMicrocharts() // Enable Microcharts for weight tracking
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register Services
            builder.Services.AddSingleton<HttpClient>();
            builder.Services.AddSingleton<INavigationService, NavigationService>();
            builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
            builder.Services.AddSingleton<IPetService, PetService>();
            builder.Services.AddSingleton<INotificationService, NotificationService>();

            builder.Services.AddSingleton<IAdService, AdService>();
            builder.Services.AddSingleton<IVeterinaryService, VeterinaryService>();
            builder.Services.AddSingleton<IVeterinarianService, VeterinarianService>();
            builder.Services.AddSingleton<IBathService, BathService>(); // Bath Control Service

            // ============ Nuevos Servicios V2.0 ============
            builder.Services.AddSingleton<IHistorialClinicoService, HistorialClinicoService>(); // Historial Clínico / Consultas
            builder.Services.AddSingleton<INotificacionService, NotificacionService>(); // Notificaciones y Recordatorios
            builder.Services.AddSingleton<IChatService, ChatService>(); // Chat / Mensajería
            builder.Services.AddSingleton<IResenaService, ResenaService>(); // Reseñas / Calificaciones

            // ============ Video Call Services ============
            builder.Services.AddSingleton<SignalRService>(); // SignalR for video call signaling

            // Register ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<PetsListViewModel>();
            builder.Services.AddTransient<PetDetailViewModel>();
            builder.Services.AddTransient<SettingsViewModel>();
            builder.Services.AddTransient<PetProfileViewModel>();
            builder.Services.AddTransient<BathControlViewModel>(); // Bath Control ViewModel

            builder.Services.AddTransient<VeterinaryDirectoryViewModel>();
            builder.Services.AddTransient<ForgotPasswordViewModel>();
            builder.Services.AddTransient<ResetPasswordViewModel>();

            // Register Pages
            builder.Services.AddTransient<DashboardPage>();
            builder.Services.AddTransient<PetDetailPage>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<BathControlPage>(); // Bath Control Page
            builder.Services.AddTransient<FrontEndHealthPets.Paginas.FlyPaginas.PetProfilePage>();

            builder.Services.AddTransient<FrontEndHealthPets.Paginas.VeterinaryDirectoryPage>();
            builder.Services.AddTransient<FrontEndHealthPets.Paginas.VeterinariansPage>();
            builder.Services.AddTransient<FrontEndHealthPets.Paginas.VeterinarianProfilePage>();
            builder.Services.AddTransient<FrontEndHealthPets.Paginas.MyAppointmentsPage>();
            builder.Services.AddTransient<FrontEndHealthPets.Paginas.ForgotPasswordPage>();
            builder.Services.AddTransient<FrontEndHealthPets.Paginas.ResetPasswordPage>();
            builder.Services.AddTransient<FrontEndHealthPets.Paginas.VaccinationPage>();
            builder.Services.AddTransient<FrontEndHealthPets.Paginas.MedicationPage>(); // Medication Control Page
            builder.Services.AddTransient<FrontEndHealthPets.Paginas.WeightTrackingPage>(); // Weight Tracking Page

            builder.Services.AddTransient<VeterinarianProfileViewModel>();
            builder.Services.AddTransient<VaccinationViewModel>();
            builder.Services.AddTransient<MedicationViewModel>(); // Medication Control ViewModel
            builder.Services.AddTransient<WeightTrackingViewModel>(); // Weight Tracking ViewModel
            builder.Services.AddTransient<MyAppointmentsViewModel>();
            builder.Services.AddTransient<VeterinariansViewModel>();
            builder.Services.AddTransient<VetProfileEditViewModel>();
            builder.Services.AddTransient<VetScheduleManagementViewModel>();

            // Vet Portal Pages
            builder.Services.AddTransient<FrontEndHealthPets.Paginas.VetProfileEditPage>();
            builder.Services.AddTransient<FrontEndHealthPets.Paginas.VetScheduleManagementPage>();

            // ============ Nuevas Páginas V2.0 ============
            builder.Services.AddTransient<HistorialClinicoViewModel>();
            builder.Services.AddTransient<ChatViewModel>();
            builder.Services.AddTransient<NotificacionesViewModel>();
            builder.Services.AddTransient<ResenasViewModel>();

            builder.Services.AddTransient<FrontEndHealthPets.Paginas.HistorialClinicoPage>();
            builder.Services.AddTransient<FrontEndHealthPets.Paginas.ChatPage>();
            builder.Services.AddTransient<FrontEndHealthPets.Paginas.NotificacionesPage>();
            builder.Services.AddTransient<FrontEndHealthPets.Paginas.ResenasPage>();

            // ============ Video Call Page ============
            builder.Services.AddTransient<VideoCallViewModel>();
            builder.Services.AddTransient<FrontEndHealthPets.Paginas.VideoCallPage>();

            // ============ Feature: Recordatorios ============
            builder.Services.AddSingleton<IRecordatorioService, RecordatorioService>();
            builder.Services.AddTransient<RecordatoriosViewModel>();
            builder.Services.AddTransient<FrontEndHealthPets.Paginas.RecordatoriosPage>();

            // ============ Feature: Clinics ============
            builder.Services.AddSingleton<IClinicService, ClinicService>();
            builder.Services.AddTransient<ClinicsViewModel>();
            builder.Services.AddTransient<FrontEndHealthPets.Paginas.ClinicsPage>();

            // ============ Vet Portal Services and Pages ============
            builder.Services.AddSingleton<IVetAuthenticationService, VetAuthenticationService>();

            // Vet Portal ViewModels
            builder.Services.AddTransient<ViewModels.VetPortal.VetLoginViewModel>();
            builder.Services.AddTransient<ViewModels.VetPortal.VetDashboardViewModel>();
            builder.Services.AddTransient<ViewModels.VetPortal.VetMyAppointmentsViewModel>();

            // Vet Portal Pages
            builder.Services.AddTransient<Paginas.VetPortal.VetLoginPage>();
            builder.Services.AddTransient<Paginas.VetPortal.VetMainPage>();
            builder.Services.AddTransient<Paginas.VetPortal.VetDashboardPage>();
            builder.Services.AddTransient<Paginas.VetPortal.VetMyAppointmentsPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();
            
            // Initialize ServiceHelper
            FrontEndHealthPets.Helpers.ServiceHelper.Initialize(app.Services);

            return app;
        }
    }
}
