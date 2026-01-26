using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FrontEndHealthPets.Models;
using FrontEndHealthPets.Services;
using System.Collections.ObjectModel;

namespace FrontEndHealthPets.ViewModels
{
    public partial class RecordatoriosViewModel : ObservableObject
    {
        private readonly IRecordatorioService _recordatorioService;
        private readonly IPetService _petService;

        [ObservableProperty]
        private ObservableCollection<Recordatorio> recordatorios = new();

        [ObservableProperty]
        private ObservableCollection<Pet> pets = new();

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private bool isRefreshing;

        [ObservableProperty]
        private bool showCreateForm;

        // Form fields
        [ObservableProperty]
        private string newTitle = "";

        [ObservableProperty]
        private string newMessage = "";

        [ObservableProperty]
        private string selectedType = "GENERAL";

        [ObservableProperty]
        private DateTime selectedDate = DateTime.Now.AddDays(1);

        [ObservableProperty]
        private TimeSpan selectedTime = new TimeSpan(9, 0, 0);

        [ObservableProperty]
        private Pet? selectedPet;

        [ObservableProperty]
        private bool isRecurring;

        [ObservableProperty]
        private string? recurrencePattern;

        public List<string> ReminderTypes { get; } = new()
        {
            "GENERAL",
            "VACUNA",
            "CITA",
            "MEDICAMENTO"
        };

        public List<string> RecurrencePatterns { get; } = new()
        {
            "DAILY",
            "WEEKLY",
            "MONTHLY"
        };

        public RecordatoriosViewModel(IRecordatorioService recordatorioService, IPetService petService)
        {
            _recordatorioService = recordatorioService;
            _petService = petService;
        }

        [RelayCommand]
        private async Task LoadRecordatoriosAsync()
        {
            if (IsLoading)
                return;

            try
            {
                IsLoading = true;

                // Load pets for the picker
                var petsResult = await _petService.GetPetsAsync();
                Pets = new ObservableCollection<Pet>(petsResult);

                // Load reminders
                var result = await _recordatorioService.GetRecordatoriosAsync();
                Recordatorios = new ObservableCollection<Recordatorio>(result);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading recordatorios: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "No se pudieron cargar los recordatorios", "OK");
            }
            finally
            {
                IsLoading = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private async Task RefreshAsync()
        {
            IsRefreshing = true;
            await LoadRecordatoriosAsync();
        }

        [RelayCommand]
        private void ToggleCreateForm()
        {
            ShowCreateForm = !ShowCreateForm;
            if (ShowCreateForm)
            {
                // Reset form
                NewTitle = "";
                NewMessage = "";
                SelectedType = "GENERAL";
                SelectedDate = DateTime.Now.AddDays(1);
                SelectedTime = new TimeSpan(9, 0, 0);
                SelectedPet = null;
                IsRecurring = false;
                RecurrencePattern = null;
            }
        }

        [RelayCommand]
        private async Task CreateRecordatorioAsync()
        {
            if (string.IsNullOrWhiteSpace(NewTitle))
            {
                await Shell.Current.DisplayAlert("Error", "El título es requerido", "OK");
                return;
            }

            try
            {
                IsLoading = true;

                var reminderDateTime = SelectedDate.Date.Add(SelectedTime);

                var request = new CreateRecordatorioRequest
                {
                    Title = NewTitle,
                    Message = NewMessage,
                    Type = SelectedType,
                    ReminderDate = reminderDateTime,
                    PetId = SelectedPet?.Id != null ? (int)SelectedPet.Id : null,
                    Recurring = IsRecurring,
                    RecurrencePattern = IsRecurring ? RecurrencePattern : null
                };

                var (success, recordatorio, error) = await _recordatorioService.CreateRecordatorioAsync(request);

                if (success && recordatorio != null)
                {
                    Recordatorios.Insert(0, recordatorio);
                    ShowCreateForm = false;
                    await Shell.Current.DisplayAlert("Éxito", "Recordatorio creado correctamente", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", error, "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al crear recordatorio: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task CancelRecordatorioAsync(Recordatorio recordatorio)
        {
            if (recordatorio == null)
                return;

            var confirm = await Shell.Current.DisplayAlert(
                "Confirmar",
                $"¿Deseas cancelar el recordatorio '{recordatorio.Title}'?",
                "Sí", "No");

            if (!confirm)
                return;

            try
            {
                IsLoading = true;
                var success = await _recordatorioService.CancelRecordatorioAsync(recordatorio.Id);

                if (success)
                {
                    Recordatorios.Remove(recordatorio);
                    await Shell.Current.DisplayAlert("Éxito", "Recordatorio cancelado", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "No se pudo cancelar el recordatorio", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public string GetTypeIcon(string type)
        {
            return type switch
            {
                "VACUNA" => "💉",
                "CITA" => "📅",
                "MEDICAMENTO" => "💊",
                _ => "🔔"
            };
        }

        public Color GetTypeColor(string type)
        {
            return type switch
            {
                "VACUNA" => Color.FromArgb("#4CAF50"),
                "CITA" => Color.FromArgb("#2196F3"),
                "MEDICAMENTO" => Color.FromArgb("#FF9800"),
                _ => Color.FromArgb("#6B4EE6")
            };
        }
    }
}
