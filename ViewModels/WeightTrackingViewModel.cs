using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FrontEndHealthPets.Models;
using FrontEndHealthPets.Services;
using Microcharts;
using SkiaSharp;
using System.Collections.ObjectModel;

namespace FrontEndHealthPets.ViewModels
{
    public partial class WeightTrackingViewModel : ObservableObject
    {
        private readonly IPetService _petService;
        private List<WeightRecord> _allWeightRecords = new();
        private bool _mockDataLoaded = false;

        [ObservableProperty]
        private ObservableCollection<Pet> pets = new();

        [ObservableProperty]
        private Pet? selectedPet;

        [ObservableProperty]
        private ObservableCollection<WeightRecord> weightRecords = new();

        [ObservableProperty]
        private WeightStats? stats;

        [ObservableProperty]
        private Chart? weightChart;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private bool hasNoRecords;

        [ObservableProperty]
        private bool showAddForm;

        [ObservableProperty]
        private double newWeight;

        [ObservableProperty]
        private DateTime newDate = DateTime.Today;

        [ObservableProperty]
        private string? newNotes;

        public WeightTrackingViewModel(IPetService petService)
        {
            _petService = petService;
            LoadDataAsync();
        }

        private async void LoadDataAsync()
        {
            IsLoading = true;

            try
            {
                var userPets = await _petService.GetPetsAsync();
                if (userPets != null && userPets.Count > 0)
                {
                    Pets.Clear();
                    foreach (var pet in userPets)
                    {
                        Pets.Add(pet);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading pets: {ex.Message}");
            }

            // Si no hay mascotas, agregar mock
            if (Pets.Count == 0)
            {
                Pets.Add(new Pet { Id = 1, Name = "Max", Species = "Perro" });
                Pets.Add(new Pet { Id = 2, Name = "Luna", Species = "Gato" });
            }

            // Cargar datos mock solo una vez
            if (!_mockDataLoaded)
            {
                LoadMockWeightRecords();
                _mockDataLoaded = true;
            }

            // Seleccionar primera mascota
            SelectedPet = Pets.First();

            IsLoading = false;
        }

        partial void OnSelectedPetChanged(Pet? value)
        {
            if (value != null)
            {
                FilterAndCalculateStats();
            }
        }

        private void LoadMockWeightRecords()
        {
            _allWeightRecords.Clear();

            // Usar los IDs reales de las mascotas cargadas
            if (Pets.Count == 0) return;

            var pet1 = Pets[0];
            var pet2 = Pets.Count > 1 ? Pets[1] : null;

            // Registros para primera mascota (Perro ~25kg)
            _allWeightRecords.AddRange(new List<WeightRecord>
            {
                new() { PetId = pet1.Id, WeightKg = 24.5, RecordDate = DateTime.Today.AddMonths(-6), Notes = "Peso inicial" },
                new() { PetId = pet1.Id, WeightKg = 25.2, RecordDate = DateTime.Today.AddMonths(-5), Notes = "Control mensual" },
                new() { PetId = pet1.Id, WeightKg = 26.0, RecordDate = DateTime.Today.AddMonths(-4), Notes = "Subió un poco" },
                new() { PetId = pet1.Id, WeightKg = 25.8, RecordDate = DateTime.Today.AddMonths(-3), Notes = "Dieta ajustada" },
                new() { PetId = pet1.Id, WeightKg = 25.3, RecordDate = DateTime.Today.AddMonths(-2), Notes = "Mejorando" },
                new() { PetId = pet1.Id, WeightKg = 25.0, RecordDate = DateTime.Today.AddMonths(-1), Notes = "Peso ideal" },
                new() { PetId = pet1.Id, WeightKg = 24.8, RecordDate = DateTime.Today.AddDays(-7), Notes = "Control reciente" },
            });

            // Registros para segunda mascota (Gato ~5kg) si existe
            if (pet2 != null)
            {
                _allWeightRecords.AddRange(new List<WeightRecord>
                {
                    new() { PetId = pet2.Id, WeightKg = 4.2, RecordDate = DateTime.Today.AddMonths(-4), Notes = "Primera visita" },
                    new() { PetId = pet2.Id, WeightKg = 4.5, RecordDate = DateTime.Today.AddMonths(-3), Notes = "Creciendo bien" },
                    new() { PetId = pet2.Id, WeightKg = 4.8, RecordDate = DateTime.Today.AddMonths(-2) },
                    new() { PetId = pet2.Id, WeightKg = 5.0, RecordDate = DateTime.Today.AddMonths(-1), Notes = "Peso saludable" },
                    new() { PetId = pet2.Id, WeightKg = 5.1, RecordDate = DateTime.Today.AddDays(-5) },
                });
            }
        }

        private void FilterAndCalculateStats()
        {
            if (SelectedPet == null)
            {
                WeightRecords.Clear();
                HasNoRecords = true;
                WeightChart = null;
                Stats = null;
                return;
            }

            // Filtrar SOLO por la mascota seleccionada
            var filtered = _allWeightRecords
                .Where(w => w.PetId == SelectedPet.Id)
                .OrderByDescending(w => w.RecordDate)
                .ToList();

            WeightRecords.Clear();
            foreach (var record in filtered)
            {
                WeightRecords.Add(record);
            }

            HasNoRecords = WeightRecords.Count == 0;

            if (!HasNoRecords)
            {
                CalculateStats(filtered);
                CreateChart(filtered);
            }
            else
            {
                Stats = null;
                WeightChart = null;
            }
        }

        private void CalculateStats(List<WeightRecord> records)
        {
            var orderedByDate = records.OrderBy(r => r.RecordDate).ToList();
            var weights = orderedByDate.Select(r => r.WeightKg).ToList();

            var currentWeight = orderedByDate.Last().WeightKg;
            var previousWeight = orderedByDate.Count > 1
                ? orderedByDate[orderedByDate.Count - 2].WeightKg
                : currentWeight;

            var change = currentWeight - previousWeight;
            string trend = "stable";
            if (change > 0.2) trend = "up";
            else if (change < -0.2) trend = "down";

            Stats = new WeightStats
            {
                CurrentWeight = currentWeight,
                MinWeight = weights.Min(),
                MaxWeight = weights.Max(),
                AverageWeight = weights.Average(),
                WeightChange = change,
                WeightTrend = trend
            };
        }

        private void CreateChart(List<WeightRecord> records)
        {
            // Ordenar por fecha y tomar los últimos 6 registros para mejor visualización
            var orderedByDate = records
                .OrderBy(r => r.RecordDate)
                .TakeLast(6)
                .ToList();

            if (orderedByDate.Count == 0) return;

            // Calcular rango para la gráfica
            var minWeight = orderedByDate.Min(r => r.WeightKg);
            var maxWeight = orderedByDate.Max(r => r.WeightKg);
            var padding = Math.Max(0.5, (maxWeight - minWeight) * 0.2);

            // Colores degradados para las barras
            var colors = new[]
            {
                SKColor.Parse("#667eea"),
                SKColor.Parse("#764ba2"),
                SKColor.Parse("#667eea"),
                SKColor.Parse("#764ba2"),
                SKColor.Parse("#667eea"),
                SKColor.Parse("#764ba2"),
            };

            var entries = orderedByDate.Select((r, index) => new ChartEntry((float)r.WeightKg)
            {
                Label = r.RecordDate.ToString("MMM"),
                ValueLabel = $"{r.WeightKg:F1}",
                Color = colors[index % colors.Length],
                ValueLabelColor = SKColor.Parse("#333333")
            }).ToList();

            // Usar BarChart para mejor visualización
            WeightChart = new BarChart
            {
                Entries = entries,
                BackgroundColor = SKColors.Transparent,
                LabelTextSize = 32,
                ValueLabelTextSize = 28,
                LabelColor = SKColor.Parse("#636E72"),
                ValueLabelOrientation = Orientation.Horizontal,
                LabelOrientation = Orientation.Horizontal,
                Margin = 20,
                AnimationDuration = TimeSpan.FromMilliseconds(500),
                MinValue = (float)(minWeight - padding),
                MaxValue = (float)(maxWeight + padding),
                BarAreaAlpha = 0
            };
        }

        [RelayCommand]
        private void ToggleAddForm()
        {
            ShowAddForm = !ShowAddForm;
            if (ShowAddForm)
            {
                NewWeight = Stats?.CurrentWeight ?? 0;
                NewDate = DateTime.Today;
                NewNotes = string.Empty;
            }
        }

        [RelayCommand]
        private async Task AddWeightRecord()
        {
            if (SelectedPet == null || NewWeight <= 0)
            {
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Error",
                        "Por favor ingresa un peso válido",
                        "OK");
                }
                return;
            }

            var newRecord = new WeightRecord
            {
                PetId = SelectedPet.Id,
                WeightKg = NewWeight,
                RecordDate = NewDate,
                Notes = NewNotes
            };

            _allWeightRecords.Add(newRecord);

            ShowAddForm = false;
            FilterAndCalculateStats();

            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Registro Agregado",
                    $"Se registró {NewWeight:F1} kg para {SelectedPet.Name}",
                    "OK");
            }
        }

        [RelayCommand]
        private async Task DeleteRecord(WeightRecord record)
        {
            if (record == null) return;

            bool confirm = false;
            if (Application.Current?.MainPage != null)
            {
                confirm = await Application.Current.MainPage.DisplayAlert(
                    "Eliminar Registro",
                    $"¿Eliminar el registro de {record.WeightFormatted} del {record.DateFormatted}?",
                    "Eliminar",
                    "Cancelar");
            }

            if (confirm)
            {
                _allWeightRecords.Remove(record);
                FilterAndCalculateStats();
            }
        }

        [RelayCommand]
        private async Task RefreshData()
        {
            IsLoading = true;
            await Task.Delay(300);
            FilterAndCalculateStats();
            IsLoading = false;
        }
    }
}
