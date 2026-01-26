using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FrontEndHealthPets.Services;
using System.Collections.ObjectModel;

namespace FrontEndHealthPets.ViewModels
{
    public partial class VetScheduleManagementViewModel : ObservableObject
    {
        private readonly IVeterinarianService _veterinarianService;
        private int _vetId;

        // Days in Spanish
        public List<string> DayOptions { get; } = new()
        {
            "Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado"
        };

        public ObservableCollection<ScheduleDisplayItem> Schedules { get; } = new();

        [ObservableProperty]
        private int selectedDayIndex = 1; // Default to Monday

        [ObservableProperty]
        private TimeSpan startTime = new TimeSpan(8, 0, 0);

        [ObservableProperty]
        private TimeSpan endTime = new TimeSpan(17, 0, 0);

        [ObservableProperty]
        private string slotDuration = "30";

        [ObservableProperty]
        private bool isLoading;

        public VetScheduleManagementViewModel(IVeterinarianService veterinarianService)
        {
            _veterinarianService = veterinarianService;
        }

        public void Initialize(int vetId)
        {
            _vetId = vetId;
            LoadSchedulesCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadSchedules()
        {
            if (IsLoading || _vetId <= 0) return;

            try
            {
                IsLoading = true;
                Schedules.Clear();

                var schedules = await _veterinarianService.GetSchedulesAsync(_vetId);
                
                foreach (var schedule in schedules)
                {
                    Schedules.Add(new ScheduleDisplayItem
                    {
                        Id = schedule.Id,
                        DayOfWeek = schedule.DayOfWeek,
                        DayName = DayOptions[schedule.DayOfWeek],
                        StartTime = schedule.StartTime,
                        EndTime = schedule.EndTime,
                        SlotDurationMinutes = schedule.SlotDurationMinutes,
                        TimeRange = $"{schedule.StartTime:hh\\:mm} - {schedule.EndTime:hh\\:mm}",
                        SlotInfo = $"Citas de {schedule.SlotDurationMinutes} minutos"
                    });
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudieron cargar los horarios: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task AddSchedule()
        {
            if (!int.TryParse(SlotDuration, out int duration) || duration < 10)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "La duración debe ser al menos 10 minutos", "OK");
                return;
            }

            if (EndTime <= StartTime)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "La hora de fin debe ser posterior a la de inicio", "OK");
                return;
            }

            try
            {
                IsLoading = true;

                var (success, error) = await _veterinarianService.CreateScheduleAsync(
                    _vetId, SelectedDayIndex, StartTime, EndTime, duration);

                if (success)
                {
                    await Application.Current.MainPage.DisplayAlert("Éxito", "Horario agregado", "OK");
                    await LoadSchedules();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", error ?? "No se pudo agregar el horario", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task DeleteSchedule(ScheduleDisplayItem schedule)
        {
            if (schedule == null) return;

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Confirmar",
                $"¿Eliminar el horario del {schedule.DayName}?",
                "Sí", "No");

            if (!confirm) return;

            try
            {
                IsLoading = true;

                var success = await _veterinarianService.DeleteScheduleAsync(schedule.Id);

                if (success)
                {
                    Schedules.Remove(schedule);
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "No se pudo eliminar el horario", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

    public class ScheduleDisplayItem
    {
        public int Id { get; set; }
        public int DayOfWeek { get; set; }
        public string DayName { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int SlotDurationMinutes { get; set; }
        public string TimeRange { get; set; }
        public string SlotInfo { get; set; }
    }
}
