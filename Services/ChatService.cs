using FrontEndHealthPets.Models;
using FrontEndHealthPets.Helpers;

namespace FrontEndHealthPets.Services
{
    /// <summary>
    /// Servicio para gestión de chat/mensajería
    /// </summary>
    public class ChatService : BaseApiService, IChatService
    {
        public ChatService() : base()
        {
        }

        private string GetFullImageUrl(string? relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return string.Empty;
            if (relativePath.StartsWith("http")) return relativePath;

            var baseUrl = Constants.ApiBaseUrl.Replace("api/", "", StringComparison.OrdinalIgnoreCase);
            return $"{baseUrl.TrimEnd('/')}/{relativePath.TrimStart('/')}";
        }

        /// <summary>
        /// Obtiene las conversaciones del usuario
        /// </summary>
        public async Task<List<Conversacion>> GetConversacionesAsync()
        {
            try
            {
                var response = await GetAsync<List<ConversacionDto>>("Chat/conversaciones");
                return response?.Select(MapConversacionFromDto).ToList() ?? new();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting conversaciones: {ex.Message}");
                return new();
            }
        }

        /// <summary>
        /// Obtiene una conversación específica
        /// </summary>
        public async Task<Conversacion?> GetConversacionAsync(int id)
        {
            try
            {
                var response = await GetAsync<ConversacionDto>($"Chat/conversaciones/{id}");
                return response != null ? MapConversacionFromDto(response) : null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting conversacion: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Inicia una nueva conversación con un veterinario
        /// </summary>
        public async Task<(bool success, Conversacion? conversacion, string error)> CreateConversacionAsync(
            int veterinarianId, int? petId = null, int? consultaId = null, string? reason = null)
        {
            try
            {
                var request = new
                {
                    VeterinarianId = veterinarianId,
                    PetId = petId,
                    ConsultaId = consultaId,
                    Reason = reason
                };

                var response = await PostAsync<object, ConversacionDto>("Chat/conversaciones", request);
                if (response == null)
                    return (false, null, "Error al iniciar conversación");

                return (true, MapConversacionFromDto(response), string.Empty);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating conversacion: {ex.Message}");
                return (false, null, ex.Message);
            }
        }

        /// <summary>
        /// Cierra una conversación
        /// </summary>
        public async Task<bool> CloseConversacionAsync(int id)
        {
            try
            {
                var response = await PostAsync<object, object>($"Chat/conversaciones/{id}/close", new { });
                return response != null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error closing conversacion: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Obtiene los mensajes de una conversación
        /// </summary>
        public async Task<List<Mensaje>> GetMensajesAsync(int conversacionId, int page = 1, int pageSize = 50)
        {
            try
            {
                var endpoint = $"Chat/conversaciones/{conversacionId}/mensajes?page={page}&pageSize={pageSize}";
                var response = await GetAsync<List<MensajeDto>>(endpoint);
                return response?.Select(MapMensajeFromDto).ToList() ?? new();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting mensajes: {ex.Message}");
                return new();
            }
        }

        /// <summary>
        /// Envía un mensaje de texto
        /// </summary>
        public async Task<(bool success, Mensaje? mensaje, string error)> SendMensajeAsync(int conversacionId, string content)
        {
            try
            {
                var request = new
                {
                    ConversacionId = conversacionId,
                    Content = content,
                    ContentType = "TEXTO"
                };

                var response = await PostAsync<object, MensajeDto>("Chat/mensajes", request);
                if (response == null)
                    return (false, null, "Error al enviar mensaje");

                return (true, MapMensajeFromDto(response), string.Empty);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error sending mensaje: {ex.Message}");
                return (false, null, ex.Message);
            }
        }

        /// <summary>
        /// Envía un mensaje con archivo adjunto
        /// </summary>
        public async Task<(bool success, Mensaje? mensaje, string error)> SendMensajeWithFileAsync(
            int conversacionId, Stream fileStream, string fileName, string? content = null)
        {
            try
            {
                var additionalData = new Dictionary<string, string>
                {
                    { "conversacionId", conversacionId.ToString() }
                };

                if (!string.IsNullOrEmpty(content))
                    additionalData.Add("content", content);

                var response = await UploadFileAsync<MensajeDto>("Chat/mensajes/with-file", fileStream, fileName, additionalData);
                if (response == null)
                    return (false, null, "Error al enviar mensaje con archivo");

                return (true, MapMensajeFromDto(response), string.Empty);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error sending mensaje with file: {ex.Message}");
                return (false, null, ex.Message);
            }
        }

        /// <summary>
        /// Marca los mensajes de una conversación como leídos
        /// </summary>
        public async Task<bool> MarkMessagesAsReadAsync(int conversacionId)
        {
            try
            {
                var request = new { ConversacionId = conversacionId };
                var response = await PostAsync<object, object>("Chat/mensajes/mark-read", request);
                return response != null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error marking messages as read: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Obtiene el conteo total de mensajes no leídos
        /// </summary>
        public async Task<int> GetUnreadMessagesCountAsync()
        {
            try
            {
                var response = await GetAsync<int>("Chat/unread-count");
                return response;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting unread count: {ex.Message}");
                return 0;
            }
        }

        #region Mappers

        private Conversacion MapConversacionFromDto(ConversacionDto dto)
        {
            return new Conversacion
            {
                Id = dto.Id,
                UserId = dto.UserId,
                UserName = dto.UserName,
                VeterinarianId = dto.VeterinarianId,
                VeterinarianName = dto.VeterinarianName,
                VeterinarianImage = GetFullImageUrl(dto.VeterinarianImage),
                PetId = dto.PetId,
                PetName = dto.PetName,
                Status = dto.Status,
                Reason = dto.Reason,
                UnreadMessages = dto.UnreadMessages,
                StartDate = dto.StartDate,
                LastMessageDate = dto.LastMessageDate,
                LastMessage = dto.LastMessage != null ? MapMensajeFromDto(dto.LastMessage) : null
            };
        }

        private Mensaje MapMensajeFromDto(MensajeDto dto)
        {
            return new Mensaje
            {
                Id = dto.Id,
                ConversacionId = dto.ConversacionId,
                SenderType = dto.SenderType,
                SenderId = dto.SenderId,
                SenderName = dto.SenderName,
                Content = dto.Content,
                ContentType = dto.ContentType,
                FileUrl = GetFullImageUrl(dto.FileUrl),
                FileName = dto.FileName,
                IsRead = dto.IsRead,
                SentDate = dto.SentDate
            };
        }

        #endregion
    }

    #region DTOs

    public class ConversacionDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = "";
        public int VeterinarianId { get; set; }
        public string VeterinarianName { get; set; } = "";
        public string? VeterinarianImage { get; set; }
        public int? PetId { get; set; }
        public string? PetName { get; set; }
        public string Status { get; set; } = "";
        public string? Reason { get; set; }
        public int UnreadMessages { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? LastMessageDate { get; set; }
        public MensajeDto? LastMessage { get; set; }
    }

    public class MensajeDto
    {
        public int Id { get; set; }
        public int ConversacionId { get; set; }
        public string SenderType { get; set; } = "";
        public int SenderId { get; set; }
        public string? SenderName { get; set; }
        public string? Content { get; set; }
        public string ContentType { get; set; } = "TEXTO";
        public string? FileUrl { get; set; }
        public string? FileName { get; set; }
        public bool IsRead { get; set; }
        public DateTime SentDate { get; set; }
    }

    #endregion
}
