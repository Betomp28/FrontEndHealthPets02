using System.Collections.ObjectModel;
using System.Windows.Input;
using FrontEndHealthPets.Helpers;
using FrontEndHealthPets.Services;

namespace FrontEndHealthPets.ViewModels.VetPortal
{
    public class VetMyAppointmentsViewModel : BaseViewModel
    {
        private readonly IVeterinarianService _vetService;
        private string _currentFilter = "";
        private bool _isRefreshing;

        public VetMyAppointmentsViewModel(IVeterinarianService vetService)
        {
            _vetService = vetService;
            Title = "Mis Citas";
            Appointments = new ObservableCollection<AppointmentItem>();

            LoadDataCommand = new Command(async () => await LoadDataAsync());
            RefreshCommand = new Command(async () => await RefreshAsync());
            FilterCommand = new Command<string>(async (filter) => await FilterAsync(filter));
            ConfirmCommand = new Command<int>(async (id) => await UpdateStatusAsync(id, "Confirmed"));
            CompleteCommand = new Command<int>(async (id) => await UpdateStatusAsync(id, "Completed"));
            CancelCommand = new Command<int>(async (id) => await UpdateStatusAsync(id, "Cancelled"));
        }

        public ObservableCollection<AppointmentItem> Appointments { get; }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        // Filter button colors
        public Color AllFilterColor => _currentFilter == "" ? Color.FromArgb("#6B4EE6") : Color.FromArgb("#252542");
        public Color PendingFilterColor => _currentFilter == "Pending" ? Color.FromArgb("#FF9F43") : Color.FromArgb("#252542");
        public Color ConfirmedFilterColor => _currentFilter == "Confirmed" ? Color.FromArgb("#10AC84") : Color.FromArgb("#252542");
        public Color CompletedFilterColor => _currentFilter == "Completed" ? Color.FromArgb("#6B4EE6") : Color.FromArgb("#252542");

        public ICommand LoadDataCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand FilterCommand { get; }
        public ICommand ConfirmCommand { get; }
        public ICommand CompleteCommand { get; }
        public ICommand CancelCommand { get; }

        private async Task LoadDataAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var appointments = await _vetService.GetVetAppointmentsAsync(Settings.VetId);

                Appointments.Clear();

                var filtered = appointments ?? Enumerable.Empty<Models.VetAppointment>();
                
                if (!string.IsNullOrEmpty(_currentFilter))
                {
                    filtered = filtered.Where(a => a.Status == _currentFilter);
                }

                foreach (var apt in filtered.OrderByDescending(a => a.AppointmentDateTime))
                {
                    Appointments.Add(new AppointmentItem
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[VetAppointments] Error: {ex.Message}");
                await ShowAlert("Error", "No se pudieron cargar las citas");
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

        private async Task FilterAsync(string filter)
        {
            _currentFilter = filter;
            OnPropertyChanged(nameof(AllFilterColor));
            OnPropertyChanged(nameof(PendingFilterColor));
            OnPropertyChanged(nameof(ConfirmedFilterColor));
            OnPropertyChanged(nameof(CompletedFilterColor));
            await LoadDataAsync();
        }

        private async Task UpdateStatusAsync(int appointmentId, string newStatus)
        {
            try
            {
                var success = await _vetService.UpdateAppointmentStatusAsync(appointmentId, newStatus);
                if (success)
                {
                    await ShowAlert("Éxito", $"Cita {GetStatusDisplayName(newStatus).ToLower()}");
                    await LoadDataAsync();
                }
                else
                {
                    await ShowAlert("Error", "No se pudo actualizar la cita");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[UpdateStatus] Error: {ex.Message}");
                await ShowAlert("Error", "Error al actualizar la cita");
            }
        }

        private string GetStatusDisplayName(string status) => status switch
        {
            "Confirmed" => "Confirmada",
            "Completed" => "Completada",
            "Cancelled" => "Cancelada",
            _ => status
        };

        public class AppointmentItem
        {
            public int Id { get; set; }
            public string UserName { get; set; } = string.Empty;
            public string PetName { get; set; } = string.Empty;
            public string Reason { get; set; } = string.Empty;
            public DateTime AppointmentDateTime { get; set; }
            public string Status { get; set; } = string.Empty;

            public string AppointmentTime => AppointmentDateTime.ToString("HH:mm");
            public string AppointmentDateFormatted => AppointmentDateTime.ToString("dddd, d MMMM");

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

            public bool IsPending => Status == "Pending";
            public bool IsConfirmed => Status == "Confirmed";
            public bool CanCancel => Status == "Pending" || Status == "Confirmed";
            public bool CanTakeAction => Status != "Completed" && Status != "Cancelled";
        }
    }
}
