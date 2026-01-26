using FrontEndHealthPets.Models;

namespace FrontEndHealthPets.Services
{
    /// <summary>
    /// Interfaz para el servicio de chat/mensajería
    /// </summary>
    public interface IChatService
    {
        /// <summary>
        /// Obtiene las conversaciones del usuario
        /// </summary>
        Task<List<Conversacion>> GetConversacionesAsync();

        /// <summary>
        /// Obtiene una conversación específica
        /// </summary>
        Task<Conversacion?> GetConversacionAsync(int id);

        /// <summary>
        /// Inicia una nueva conversación con un veterinario
        /// </summary>
        Task<(bool success, Conversacion? conversacion, string error)> CreateConversacionAsync(
            int veterinarianId, int? petId = null, int? consultaId = null, string? reason = null);

        /// <summary>
        /// Cierra una conversación
        /// </summary>
        Task<bool> CloseConversacionAsync(int id);

        /// <summary>
        /// Obtiene los mensajes de una conversación
        /// </summary>
        Task<List<Mensaje>> GetMensajesAsync(int conversacionId, int page = 1, int pageSize = 50);

        /// <summary>
        /// Envía un mensaje de texto
        /// </summary>
        Task<(bool success, Mensaje? mensaje, string error)> SendMensajeAsync(int conversacionId, string content);

        /// <summary>
        /// Envía un mensaje con archivo adjunto
        /// </summary>
        Task<(bool success, Mensaje? mensaje, string error)> SendMensajeWithFileAsync(
            int conversacionId, Stream fileStream, string fileName, string? content = null);

        /// <summary>
        /// Marca los mensajes de una conversación como leídos
        /// </summary>
        Task<bool> MarkMessagesAsReadAsync(int conversacionId);

        /// <summary>
        /// Obtiene el conteo total de mensajes no leídos
        /// </summary>
        Task<int> GetUnreadMessagesCountAsync();
    }
}
