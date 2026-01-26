using FrontEndHealthPets.Models;

namespace FrontEndHealthPets.Services
{
    /// <summary>
    /// Interfaz para el servicio de notificaciones
    /// </summary>
    public interface INotificacionService
    {
        /// <summary>
        /// Obtiene las notificaciones del usuario
        /// </summary>
        Task<List<Notificacion>> GetNotificacionesAsync(bool unreadOnly = false, int page = 1, int pageSize = 20);

        /// <summary>
        /// Obtiene el conteo de notificaciones no leídas
        /// </summary>
        Task<int> GetUnreadCountAsync();

        /// <summary>
        /// Marca notificaciones como leídas
        /// </summary>
        Task<bool> MarkAsReadAsync(List<int> notificationIds);

        /// <summary>
        /// Marca todas las notificaciones como leídas
        /// </summary>
        Task<bool> MarkAllAsReadAsync();

        /// <summary>
        /// Elimina una notificación
        /// </summary>
        Task<bool> DeleteNotificacionAsync(int id);

        /// <summary>
        /// Registra un token de dispositivo para push notifications
        /// </summary>
        Task<bool> RegisterDeviceTokenAsync(string token, string platform, string? deviceModel = null, string? appVersion = null);

        /// <summary>
        /// Desactiva un token de dispositivo
        /// </summary>
        Task<bool> DeactivateDeviceTokenAsync(int tokenId);

        /// <summary>
        /// Crea un recordatorio programado
        /// </summary>
        Task<(bool success, Recordatorio? recordatorio, string error)> CreateRecordatorioAsync(RecordatorioCreateRequest request);

        /// <summary>
        /// Obtiene los recordatorios del usuario
        /// </summary>
        Task<List<Recordatorio>> GetRecordatoriosAsync(bool activeOnly = true);

        /// <summary>
        /// Cancela un recordatorio
        /// </summary>
        Task<bool> CancelRecordatorioAsync(int id);
    }
}
