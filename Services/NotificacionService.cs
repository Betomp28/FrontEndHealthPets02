using FrontEndHealthPets.Models;
using FrontEndHealthPets.Helpers;

namespace FrontEndHealthPets.Services
{
    /// <summary>
    /// Servicio para gestión de notificaciones y recordatorios
    /// </summary>
    public class NotificacionService : BaseApiService, INotificacionService
    {
        public NotificacionService() : base()
        {
        }

        /// <summary>
        /// Obtiene las notificaciones del usuario
        /// </summary>
        public async Task<List<Notificacion>> GetNotificacionesAsync(bool unreadOnly = false, int page = 1, int pageSize = 20)
        {
            try
            {
                var endpoint = $"Notificaciones?unreadOnly={unreadOnly}&page={page}&pageSize={pageSize}";
                var response = await GetAsync<List<NotificacionDto>>(endpoint);
                return response?.Select(MapNotificacionFromDto).ToList() ?? new();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting notificaciones: {ex.Message}");
                return new();
            }
        }

        /// <summary>
        /// Obtiene el conteo de notificaciones no leídas
        /// </summary>
        public async Task<int> GetUnreadCountAsync()
        {
            try
            {
                var response = await GetAsync<int>("Notificaciones/unread-count");
                return response;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting unread count: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Marca notificaciones como leídas
        /// </summary>
        public async Task<bool> MarkAsReadAsync(List<int> notificationIds)
        {
            try
            {
                var request = new { NotificationIds = notificationIds };
                var response = await PostAsync<object, object>("Notificaciones/mark-read", request);
                return response != null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error marking as read: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Marca todas las notificaciones como leídas
        /// </summary>
        public async Task<bool> MarkAllAsReadAsync()
        {
            try
            {
                var response = await PostAsync<object, object>("Notificaciones/mark-all-read", new { });
                return response != null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error marking all as read: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina una notificación
        /// </summary>
        public async Task<bool> DeleteNotificacionAsync(int id)
        {
            try
            {
                return await DeleteAsync($"Notificaciones/{id}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting notificacion: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Registra un token de dispositivo para push notifications
        /// </summary>
        public async Task<bool> RegisterDeviceTokenAsync(string token, string platform, string? deviceModel = null, string? appVersion = null)
        {
            try
            {
                var request = new
                {
                    Token = token,
                    Platform = platform,
                    DeviceModel = deviceModel,
                    AppVersion = appVersion
                };

                var response = await PostAsync<object, DeviceTokenDto>("Notificaciones/device-token", request);
                return response != null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error registering device token: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Desactiva un token de dispositivo
        /// </summary>
        public async Task<bool> DeactivateDeviceTokenAsync(int tokenId)
        {
            try
            {
                return await DeleteAsync($"Notificaciones/device-token/{tokenId}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deactivating device token: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Crea un recordatorio programado
        /// </summary>
        public async Task<(bool success, Recordatorio? recordatorio, string error)> CreateRecordatorioAsync(RecordatorioCreateRequest request)
        {
            try
            {
                var response = await PostAsync<RecordatorioCreateRequest, RecordatorioDto>("Notificaciones/recordatorios", request);
                if (response == null)
                    return (false, null, "Error al crear recordatorio");

                return (true, MapRecordatorioFromDto(response), string.Empty);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating recordatorio: {ex.Message}");
                return (false, null, ex.Message);
            }
        }

        /// <summary>
        /// Obtiene los recordatorios del usuario
        /// </summary>
        public async Task<List<Recordatorio>> GetRecordatoriosAsync(bool activeOnly = true)
        {
            try
            {
                var endpoint = $"Notificaciones/recordatorios?activeOnly={activeOnly}";
                var response = await GetAsync<List<RecordatorioDto>>(endpoint);
                return response?.Select(MapRecordatorioFromDto).ToList() ?? new();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting recordatorios: {ex.Message}");
                return new();
            }
        }

        /// <summary>
        /// Cancela un recordatorio
        /// </summary>
        public async Task<bool> CancelRecordatorioAsync(int id)
        {
            try
            {
                return await DeleteAsync($"Notificaciones/recordatorios/{id}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error canceling recordatorio: {ex.Message}");
                return false;
            }
        }

        #region Mappers

        private Notificacion MapNotificacionFromDto(NotificacionDto dto)
        {
            return new Notificacion
            {
                Id = dto.Id,
                Type = dto.Type,
                Title = dto.Title,
                Message = dto.Message,
                Icon = dto.Icon,
                IsRead = dto.IsRead,
                SentDate = dto.SentDate,
                CreatedAt = dto.CreatedAt,
                EntityType = dto.EntityType,
                EntityId = dto.EntityId
            };
        }

        private Recordatorio MapRecordatorioFromDto(RecordatorioDto dto)
        {
            return new Recordatorio
            {
                Id = dto.Id,
                PetId = dto.PetId,
                PetName = dto.PetName,
                Type = dto.Type,
                Title = dto.Title,
                Message = dto.Message,
                ReminderDate = dto.ReminderDate,
                IsRecurring = dto.IsRecurring,
                FrequencyDays = dto.FrequencyDays,
                IsActive = dto.IsActive,
                IsSent = dto.IsSent
            };
        }

        #endregion
    }

    #region DTOs

    public class NotificacionDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = "";
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public string? Icon { get; set; }
        public bool IsRead { get; set; }
        public DateTime? SentDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? EntityType { get; set; }
        public int? EntityId { get; set; }
    }

    public class DeviceTokenDto
    {
        public int Id { get; set; }
        public string Token { get; set; } = "";
        public string Platform { get; set; } = "";
        public string? DeviceModel { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class RecordatorioDto
    {
        public int Id { get; set; }
        public int? PetId { get; set; }
        public string? PetName { get; set; }
        public string Type { get; set; } = "";
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public DateTime ReminderDate { get; set; }
        public bool IsRecurring { get; set; }
        public int? FrequencyDays { get; set; }
        public bool IsActive { get; set; }
        public bool IsSent { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class RecordatorioCreateRequest
    {
        public int? PetId { get; set; }
        public string Type { get; set; } = "";
        public int? EntityId { get; set; }
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public DateTime ReminderDate { get; set; }
        public bool IsRecurring { get; set; }
        public int? FrequencyDays { get; set; }
    }

    #endregion
}
