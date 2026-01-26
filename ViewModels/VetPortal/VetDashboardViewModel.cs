using System.Collections.ObjectModel;
using System.Windows.Input;
using FrontEndHealthPets.Helpers;
using FrontEndHealthPets.Services;

namespace FrontEndHealthPets.ViewModels.VetPortal
{
    public class VetDashboardViewModel : BaseViewModel
    {
        private readonly IVetAuthenticationService _vetAuthService;
        private readonly IVeterinarianService _vetService;

        private int _todayAppointments;
        private int _pendingAppointments;
        private bool _isRefreshing;

        public VetDashboardViewModel(IVetAuthenticationService vetAuthService, IVeterinarianService vetService)
        {
            _vetAuthService = vetAuthService;
            _vetService = vetService;

            Title = "Dashboard";
            UpcomingAppointments = new ObservableCollection<AppointmentItem>();

            LoadDataCommand = new Command(async () => await LoadDataAsync());
            RefreshCommand = new Command(async () => await RefreshAsync());
            ViewAppointmentsCommand = new Command(async () => await ViewAppointmentsAsync());
            ManageScheduleCommand = new Command(async () => await ManageScheduleAsync());
        }

        public string WelcomeMessage => $"¡Hola, Dr. {Settings.VetName}!";
        public string TodayDate => DateTime.Now.ToString("dddd, d MMMM yyyy");

        public int TodayAppointments
        {
            get => _todayAppointments;
            set => SetProperty(ref _todayAppointments, value);
        }

        public int PendingAppointments
        {
            get => _pendingAppointments;
            set => SetProperty(ref _pendingAppointments, value);
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public ObservableCollection<AppointmentItem> UpcomingAppointments { get; }

        public bool HasAppointments => UpcomingAppointments.Count > 0;
        public bool HasNoAppointments => UpcomingAppointments.Count == 0;

        public ICommand LoadDataCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ViewAppointmentsCommand { get; }
        public ICommand ManageScheduleCommand { get; }

        private async Task LoadDataAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                // Get profile with stats
                var profile = await _vetAuthService.GetProfileAsync();
                if (profile != null)
                {
                    TodayAppointments = profile.TodayAppointments;
                    PendingAppointments = profile.PendingAppointments;
                }

                // Get upcoming appointments
                var appointments = await _vetService.GetVetAppointmentsAsync(Settings.VetId);
                
                UpcomingAppointments.Clear();
                
                var upcomingOnly = appointments?
                    .Where(a => a.AppointmentDateTime >= DateTime.Now && a.Status != "Cancelled")
                    .OrderBy(a => a.AppointmentDateTime)
                    .Take(5);

                if (upcomingOnly != null)
                {
                    foreach (var apt in upcomingOnly)
                    {
                        UpcomingAppointments.Add(new AppointmentItem
                        {
                            Id = apt.Id,
                            UserName = apt.UserName,
                            PetName = apt.PetName ?? "Sin mascota",
                            Reason = apt.Reason ?? "Consulta general",
                            AppointmentDateTime = apt.AppointmentDateTime,
                            Status = apt.Status
                        });
                    }
                }

                OnPropertyChanged(nameof(HasAppointments));
                OnPropertyChanged(nameof(HasNoAppointments));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[VetDashboard] Error: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task RefreshAsync()
        {
            IsRefreshing = true;
            await LoadDataAsync();
            IsRefreshing = false;
        }

        private async Task ViewAppointmentsAsync()
        {
            // This will be handled by VetMainPage navigation
            await ShowAlert("Info", "Usa el menú lateral para ver todas las citas");
        }

        private async Task ManageScheduleAsync()
        {
            // This will be handled by VetMainPage navigation
            await ShowAlert("Info", "Usa el menú lateral para gestionar horarios");
        }

        public class AppointmentItem
        {
            public int Id { get; set; }
            public string UserName { get; set; } = string.Empty;
            public string PetName { get; set; } = string.Empty;
            public string Reason { get; set; } = string.Empty;
            public DateTime AppointmentDateTime { get; set; }
            public string Status { get; set; } = string.Empty;

            public string AppointmentTime => AppointmentDateTime.ToString("HH:mm");
            public string AppointmentDateShort => AppointmentDateTime.ToString("dd/MM");
            
            public string StatusText => Status switch
            {
                "Pending" => "Pendiente",
                "Confirmed" => "Confirmada",
                "Completed" => "Completada",
                "Cancelled" => "Cancelada",
                _ => Status
            };

            public Color StatusColor => Status switch
            {
                "Pending" => Color.FromArgb("#FF9F43"),
                "Confirmed" => Color.FromArgb("#10AC84"),
                "Completed" => Color.FromArgb("#6B4EE6"),
                "Cancelled" => Color.FromArgb("#FF6B6B"),
                _ => Color.FromArgb("#888888")
            };
        }
    }
}
