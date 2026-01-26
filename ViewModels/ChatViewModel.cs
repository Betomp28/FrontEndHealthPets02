using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FrontEndHealthPets.Models;
using FrontEndHealthPets.Services;
using System.Collections.ObjectModel;

namespace FrontEndHealthPets.ViewModels
{
    public partial class ChatViewModel : ObservableObject
    {
        private readonly IChatService _chatService;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private bool isInConversation;

        [ObservableProperty]
        private Conversacion? selectedConversacion;

        [ObservableProperty]
        private string newMessage = string.Empty;

        public ObservableCollection<Conversacion> Conversaciones { get; } = new();
        public ObservableCollection<Mensaje> Mensajes { get; } = new();

        public ChatViewModel(IChatService chatService)
        {
            _chatService = chatService;
            LoadConversacionesCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadConversaciones()
        {
            if (IsLoading) return;

            try
            {
                IsLoading = true;
                Conversaciones.Clear();

                var conversaciones = await _chatService.GetConversacionesAsync();
                foreach (var conv in conversaciones.OrderByDescending(c => c.UltimoMensaje))
                {
                    Conversaciones.Add(conv);
                }
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", "No se pudieron cargar las conversaciones", "OK");
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task OpenConversacion(Conversacion conversacion)
        {
            if (conversacion == null) return;

            try
            {
                IsLoading = true;
                SelectedConversacion = conversacion;
                Mensajes.Clear();

                var mensajes = await _chatService.GetMensajesAsync(conversacion.Id);
                foreach (var msg in mensajes.OrderBy(m => m.FechaEnvio))
                {
                    Mensajes.Add(msg);
                }

                // Mark as read
                await _chatService.MarkMessagesAsReadAsync(conversacion.Id);
                conversacion.MensajesNoLeidos = 0;

                IsInConversation = true;
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", "No se pudieron cargar los mensajes", "OK");
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task SendMessage()
        {
            if (string.IsNullOrWhiteSpace(NewMessage) || SelectedConversacion == null) return;

            try
            {
                IsLoading = true;
                var messageText = NewMessage;
                NewMessage = string.Empty;

                var (success, mensaje, error) = await _chatService.SendMensajeAsync(
                    SelectedConversacion.Id,
                    messageText);

                if (success && mensaje != null)
                {
                    Mensajes.Add(mensaje);
                }
                else
                {
                    await Application.Current!.MainPage!.DisplayAlert("Error", error ?? "No se pudo enviar el mensaje", "OK");
                    NewMessage = messageText; // Restore message
                }
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", "Error al enviar mensaje", "OK");
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void BackToConversations()
        {
            IsInConversation = false;
            SelectedConversacion = null;
            Mensajes.Clear();
            LoadConversacionesCommand.Execute(null);
        }

        [RelayCommand]
        private async Task CloseConversacion()
        {
            if (SelectedConversacion == null) return;

            bool confirm = await Application.Current!.MainPage!.DisplayAlert(
                "Cerrar Conversacion",
                "Deseas cerrar esta conversacion? No podras enviar mas mensajes.",
                "Si", "No");

            if (!confirm) return;

            try
            {
                IsLoading = true;
                var success = await _chatService.CloseConversacionAsync(SelectedConversacion.Id);

                if (success)
                {
                    await Application.Current!.MainPage!.DisplayAlert("Exito", "Conversacion cerrada", "OK");
                    BackToConversationsCommand.Execute(null);
                }
                else
                {
                    await Application.Current!.MainPage!.DisplayAlert("Error", "No se pudo cerrar la conversacion", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", "Error al cerrar conversacion", "OK");
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task StartVideoCall()
        {
            if (SelectedConversacion == null) return;

            try
            {
                // Request camera and microphone permissions
                var cameraStatus = await Permissions.CheckStatusAsync<Permissions.Camera>();
                if (cameraStatus != PermissionStatus.Granted)
                {
                    cameraStatus = await Permissions.RequestAsync<Permissions.Camera>();
                    if (cameraStatus != PermissionStatus.Granted)
                    {
                        await Application.Current!.MainPage!.DisplayAlert(
                            "Permiso requerido",
                            "Se necesita acceso a la camara para realizar videollamadas",
                            "OK");
                        return;
                    }
                }

                var micStatus = await Permissions.CheckStatusAsync<Permissions.Microphone>();
                if (micStatus != PermissionStatus.Granted)
                {
                    micStatus = await Permissions.RequestAsync<Permissions.Microphone>();
                    if (micStatus != PermissionStatus.Granted)
                    {
                        await Application.Current!.MainPage!.DisplayAlert(
                            "Permiso requerido",
                            "Se necesita acceso al microfono para realizar videollamadas",
                            "OK");
                        return;
                    }
                }

                // Navigate to video call page
                var videoCallPage = Application.Current!.Handler!.MauiContext!.Services.GetService<Paginas.VideoCallPage>();
                if (videoCallPage != null)
                {
                    await Application.Current!.MainPage!.Navigation.PushModalAsync(videoCallPage);
                    await videoCallPage.InitializeCallAsync(
                        isCaller: true,
                        conversacionId: SelectedConversacion.Id,
                        participantName: SelectedConversacion.OtroParticipanteNombre);
                }
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", "No se pudo iniciar la videollamada", "OK");
                System.Diagnostics.Debug.WriteLine($"[Chat] Error starting video call: {ex.Message}");
            }
        }
    }
}
