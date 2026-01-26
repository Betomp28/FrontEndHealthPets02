using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FrontEndHealthPets.Models;
using FrontEndHealthPets.Services;
using System.Collections.ObjectModel;

namespace FrontEndHealthPets.ViewModels
{
    public partial class HistorialClinicoViewModel : ObservableObject
    {
        private readonly IHistorialClinicoService _historialService;
        private readonly IPetService _petService;
        private long _currentPetId;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private Pet? selectedPet;

        [ObservableProperty]
        private HistorialClinico? historial;

        [ObservableProperty]
        private Consulta? selectedConsulta;

        public ObservableCollection<Pet> Pets { get; } = new();
        public ObservableCollection<Consulta> Consultas { get; } = new();

        public HistorialClinicoViewModel(IHistorialClinicoService historialService, IPetService petService)
        {
            _historialService = historialService;
            _petService = petService;
            LoadPetsCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadPets()
        {
            if (IsLoading) return;

            try
            {
                IsLoading = true;
                Pets.Clear();

                var pets = await _petService.GetPetsAsync();
                foreach (var pet in pets)
                {
                    Pets.Add(pet);
                }

                if (Pets.Count > 0)
                {
                    SelectedPet = Pets[0];
                }
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", "No se pudieron cargar las mascotas", "OK");
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        partial void OnSelectedPetChanged(Pet? value)
        {
            if (value != null)
            {
                _currentPetId = value.Id;
                LoadHistorialCommand.Execute(null);
            }
        }

        [RelayCommand]
        private async Task LoadHistorial()
        {
            if (IsLoading || _currentPetId <= 0) return;

            try
            {
                IsLoading = true;
                Consultas.Clear();

                Historial = await _historialService.GetHistorialClinicoAsync(_currentPetId);
                var consultas = await _historialService.GetConsultasByPetAsync(_currentPetId);

                foreach (var consulta in consultas.OrderByDescending(c => c.FechaConsulta))
                {
                    Consultas.Add(consulta);
                }
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", "No se pudo cargar el historial clínico", "OK");
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task ViewConsultaDetail(Consulta consulta)
        {
            if (consulta == null) return;

            try
            {
                SelectedConsulta = await _historialService.GetConsultaAsync(consulta.Id);

                if (SelectedConsulta != null)
                {
                    var detail = $"Veterinario: {SelectedConsulta.VeterinarioNombre}\n" +
                                $"Fecha: {SelectedConsulta.FechaConsulta:dd/MM/yyyy}\n" +
                                $"Tipo: {SelectedConsulta.TipoConsulta}\n\n" +
                                $"Motivo: {SelectedConsulta.MotivoConsulta}\n\n" +
                                $"Diagnóstico: {SelectedConsulta.Diagnostico ?? "Sin diagnóstico"}\n\n" +
                                $"Tratamiento: {SelectedConsulta.Tratamiento ?? "Sin tratamiento"}\n\n" +
                                $"Notas: {SelectedConsulta.Notas ?? "Sin notas"}";

                    await Application.Current!.MainPage!.DisplayAlert(
                        $"Consulta - {SelectedConsulta.TipoConsulta}",
                        detail,
                        "Cerrar");
                }
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", "No se pudieron cargar los detalles", "OK");
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
    }
}
