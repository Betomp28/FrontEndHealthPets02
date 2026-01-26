using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FrontEndHealthPets.Models;
using FrontEndHealthPets.Services;
using System.Collections.ObjectModel;

namespace FrontEndHealthPets.ViewModels
{
    public partial class ResenasViewModel : ObservableObject
    {
        private readonly IResenaService _resenaService;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private bool isWritingReview;

        [ObservableProperty]
        private int selectedRating = 5;

        [ObservableProperty]
        private string reviewComment = string.Empty;

        [ObservableProperty]
        private Resena? selectedResena;

        public ObservableCollection<Resena> MisResenas { get; } = new();

        public ResenasViewModel(IResenaService resenaService)
        {
            _resenaService = resenaService;
            LoadMisResenasCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadMisResenas()
        {
            if (IsLoading) return;

            try
            {
                IsLoading = true;
                MisResenas.Clear();

                var resenas = await _resenaService.GetMyReviewsAsync();
                foreach (var resena in resenas.OrderByDescending(r => r.FechaCreacion))
                {
                    MisResenas.Add(resena);
                }
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", "No se pudieron cargar tus resenas", "OK");
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task ViewResenaDetail(Resena resena)
        {
            if (resena == null) return;

            SelectedResena = resena;

            var detail = $"Veterinario: {resena.VeterinarioNombre}\n" +
                        $"Calificacion: {"★".PadRight(resena.Calificacion, '★').PadRight(5, '☆')}\n" +
                        $"Fecha: {resena.FechaCreacion:dd/MM/yyyy}\n\n" +
                        $"Tu comentario:\n{resena.Comentario}\n";

            if (!string.IsNullOrEmpty(resena.RespuestaVeterinario))
            {
                detail += $"\nRespuesta del veterinario:\n{resena.RespuestaVeterinario}";
            }

            await Application.Current!.MainPage!.DisplayAlert("Detalle de Resena", detail, "Cerrar");
        }

        [RelayCommand]
        private async Task EditResena(Resena resena)
        {
            if (resena == null) return;

            // Cannot edit if vet has responded
            if (!string.IsNullOrEmpty(resena.RespuestaVeterinario))
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "No se puede editar",
                    "No puedes editar una resena que ya tiene respuesta del veterinario",
                    "OK");
                return;
            }

            SelectedResena = resena;
            SelectedRating = resena.Calificacion;
            ReviewComment = resena.Comentario;
            IsWritingReview = true;
        }

        [RelayCommand]
        private async Task DeleteResena(Resena resena)
        {
            if (resena == null) return;

            bool confirm = await Application.Current!.MainPage!.DisplayAlert(
                "Eliminar Resena",
                "Deseas eliminar esta resena?",
                "Si", "No");

            if (!confirm) return;

            try
            {
                IsLoading = true;
                var success = await _resenaService.DeleteResenaAsync(resena.Id);

                if (success)
                {
                    MisResenas.Remove(resena);
                    await Application.Current!.MainPage!.DisplayAlert("Exito", "Resena eliminada", "OK");
                }
                else
                {
                    await Application.Current!.MainPage!.DisplayAlert("Error", "No se pudo eliminar la resena", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", "Error al eliminar", "OK");
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task SaveResena()
        {
            if (string.IsNullOrWhiteSpace(ReviewComment))
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", "Escribe un comentario", "OK");
                return;
            }

            if (SelectedResena == null) return;

            try
            {
                IsLoading = true;

                var request = new ResenaCreateRequest
                {
                    VeterinarioId = SelectedResena.VeterinarioId,
                    Calificacion = SelectedRating,
                    Comentario = ReviewComment
                };

                var (success, error) = await _resenaService.UpdateResenaAsync(SelectedResena.Id, request);

                if (success)
                {
                    await Application.Current!.MainPage!.DisplayAlert("Exito", "Resena actualizada", "OK");
                    CancelEditCommand.Execute(null);
                    await LoadMisResenas();
                }
                else
                {
                    await Application.Current!.MainPage!.DisplayAlert("Error", error ?? "No se pudo actualizar", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", "Error al guardar", "OK");
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void CancelEdit()
        {
            IsWritingReview = false;
            SelectedResena = null;
            SelectedRating = 5;
            ReviewComment = string.Empty;
        }

        [RelayCommand]
        private void SetRating(string rating)
        {
            if (int.TryParse(rating, out var r) && r >= 1 && r <= 5)
            {
                SelectedRating = r;
            }
        }
    }
}
