using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FrontEndHealthPets.Models;
using FrontEndHealthPets.Services;
using System.Collections.ObjectModel;

namespace FrontEndHealthPets.ViewModels
{
    public partial class NotificacionesViewModel : ObservableObject
    {
        private readonly INotificacionService _notificacionService;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private int unreadCount;

        [ObservableProperty]
        private bool showUnreadOnly;

        public ObservableCollection<Notificacion> Notificaciones { get; } = new();
        public ObservableCollection<Recordatorio> Recordatorios { get; } = new();

        public NotificacionesViewModel(INotificacionService notificacionService)
        {
            _notificacionService = notificacionService;
            LoadNotificacionesCommand.Execute(null);
        }

        partial void OnShowUnreadOnlyChanged(bool value)
        {
            LoadNotificacionesCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadNotificaciones()
        {
            if (IsLoading) return;

            try
            {
                IsLoading = true;
                Notificaciones.Clear();

                var notificaciones = await _notificacionService.GetNotificacionesAsync(ShowUnreadOnly);
                foreach (var notif in notificaciones.OrderByDescending(n => n.FechaCreacion))
                {
                    Notificaciones.Add(notif);
                }

                UnreadCount = await _notificacionService.GetUnreadCountAsync();
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", "No se pudieron cargar las notificaciones", "OK");
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task LoadRecordatorios()
        {
            if (IsLoading) return;

            try
            {
                IsLoading = true;
                Recordatorios.Clear();

                var recordatorios = await _notificacionService.GetRecordatoriosAsync(true);
                foreach (var rec in recordatorios.OrderBy(r => r.FechaProgramada))
                {
                    Recordatorios.Add(rec);
                }
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", "No se pudieron cargar los recordatorios", "OK");
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task MarkAsRead(Notificacion notificacion)
        {
            if (notificacion == null || notificacion.Leida) return;

            try
            {
                var success = await _notificacionService.MarkAsReadAsync(new List<int> { notificacion.Id });
                if (success)
                {
                    notificacion.Leida = true;
                    UnreadCount = Math.Max(0, UnreadCount - 1);

                    // Refresh to update UI
                    var index = Notificaciones.IndexOf(notificacion);
                    if (index >= 0)
                    {
                        Notificaciones.RemoveAt(index);
                        Notificaciones.Insert(index, notificacion);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        [RelayCommand]
        private async Task MarkAllAsRead()
        {
            if (UnreadCount == 0) return;

            try
            {
                IsLoading = true;
                var success = await _notificacionService.MarkAllAsReadAsync();

                if (success)
                {
                    UnreadCount = 0;
                    await LoadNotificaciones();
                    await Application.Current!.MainPage!.DisplayAlert("Exito", "Todas las notificaciones marcadas como leidas", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", "No se pudieron marcar como leidas", "OK");
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task DeleteNotificacion(Notificacion notificacion)
        {
            if (notificacion == null) return;

            bool confirm = await Application.Current!.MainPage!.DisplayAlert(
                "Eliminar",
                "Deseas eliminar esta notificacion?",
                "Si", "No");

            if (!confirm) return;

            try
            {
                var success = await _notificacionService.DeleteNotificacionAsync(notificacion.Id);
                if (success)
                {
                    Notificaciones.Remove(notificacion);
                    if (!notificacion.Leida)
                        UnreadCount = Math.Max(0, UnreadCount - 1);
                }
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", "No se pudo eliminar", "OK");
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        [RelayCommand]
        private async Task CancelRecordatorio(Recordatorio recordatorio)
        {
            if (recordatorio == null) return;

            bool confirm = await Application.Current!.MainPage!.DisplayAlert(
                "Cancelar Recordatorio",
                "Deseas cancelar este recordatorio?",
                "Si", "No");

            if (!confirm) return;

            try
            {
                var success = await _notificacionService.CancelRecordatorioAsync(recordatorio.Id);
                if (success)
                {
                    Recordatorios.Remove(recordatorio);
                    await Application.Current!.MainPage!.DisplayAlert("Exito", "Recordatorio cancelado", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", "No se pudo cancelar el recordatorio", "OK");
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
    }
}
