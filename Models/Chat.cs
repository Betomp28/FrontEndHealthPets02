using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FrontEndHealthPets.Models
{
    /// <summary>
    /// Conversación de chat entre usuario y veterinario
    /// </summary>
    public class Conversacion : INotifyPropertyChanged
    {
        private int _id;
        private int _userId;
        private string _userName = string.Empty;
        private int _veterinarianId;
        private string _veterinarianName = string.Empty;
        private string? _veterinarianImage;
        private int? _petId;
        private string? _petName;
        private string _status = "ACTIVA";
        private string? _reason;
        private int _unreadMessages;
        private DateTime _startDate;
        private DateTime? _lastMessageDate;
        private Mensaje? _lastMessage;

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        public int UserId
        {
            get => _userId;
            set { _userId = value; OnPropertyChanged(); }
        }

        public string UserName
        {
            get => _userName;
            set { _userName = value; OnPropertyChanged(); }
        }

        public int VeterinarianId
        {
            get => _veterinarianId;
            set { _veterinarianId = value; OnPropertyChanged(); }
        }

        public string VeterinarianName
        {
            get => _veterinarianName;
            set { _veterinarianName = value; OnPropertyChanged(); }
        }

        public string? VeterinarianImage
        {
            get => _veterinarianImage;
            set { _veterinarianImage = value; OnPropertyChanged(); }
        }

        public int? PetId
        {
            get => _petId;
            set { _petId = value; OnPropertyChanged(); }
        }

        public string? PetName
        {
            get => _petName;
            set { _petName = value; OnPropertyChanged(); }
        }

        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        public string? Reason
        {
            get => _reason;
            set { _reason = value; OnPropertyChanged(); }
        }

        public int UnreadMessages
        {
            get => _unreadMessages;
            set { _unreadMessages = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasUnreadMessages)); }
        }

        public DateTime StartDate
        {
            get => _startDate;
            set { _startDate = value; OnPropertyChanged(); }
        }

        public DateTime? LastMessageDate
        {
            get => _lastMessageDate;
            set { _lastMessageDate = value; OnPropertyChanged(); OnPropertyChanged(nameof(LastMessageTimeAgo)); }
        }

        public Mensaje? LastMessage
        {
            get => _lastMessage;
            set { _lastMessage = value; OnPropertyChanged(); OnPropertyChanged(nameof(LastMessagePreview)); }
        }

        // Alias properties for backward compatibility (Spanish names)
        public DateTime? UltimoMensaje
        {
            get => LastMessageDate;
            set => LastMessageDate = value;
        }

        public int MensajesNoLeidos
        {
            get => UnreadMessages;
            set => UnreadMessages = value;
        }

        public string OtroParticipanteNombre => VeterinarianName;

        public string Estado
        {
            get => Status;
            set => Status = value;
        }

        public string UltimoMensajeTexto => LastMessagePreview;

        // Propiedades calculadas
        public bool HasUnreadMessages => UnreadMessages > 0;

        public string LastMessagePreview
        {
            get
            {
                if (LastMessage == null) return "Sin mensajes";
                if (LastMessage.ContentType == "IMAGEN") return "Imagen";
                if (LastMessage.ContentType == "ARCHIVO") return "Archivo";
                var content = LastMessage.Content ?? "";
                return content.Length > 50 ? content.Substring(0, 47) + "..." : content;
            }
        }

        public string LastMessageTimeAgo
        {
            get
            {
                if (!LastMessageDate.HasValue) return "";
                var diff = DateTime.UtcNow - LastMessageDate.Value;
                if (diff.TotalMinutes < 1) return "Ahora";
                if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes}m";
                if (diff.TotalHours < 24) return $"{(int)diff.TotalHours}h";
                return LastMessageDate.Value.ToString("dd/MM");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    /// <summary>
    /// Mensaje de chat
    /// </summary>
    public class Mensaje : INotifyPropertyChanged
    {
        private int _id;
        private int _conversacionId;
        private string _senderType = string.Empty;
        private int _senderId;
        private string? _senderName;
        private string? _content;
        private string _contentType = "TEXTO";
        private string? _fileUrl;
        private string? _fileName;
        private bool _isRead;
        private DateTime _sentDate;

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        public int ConversacionId
        {
            get => _conversacionId;
            set { _conversacionId = value; OnPropertyChanged(); }
        }

        public string SenderType
        {
            get => _senderType;
            set { _senderType = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsFromUser)); }
        }

        public int SenderId
        {
            get => _senderId;
            set { _senderId = value; OnPropertyChanged(); }
        }

        public string? SenderName
        {
            get => _senderName;
            set { _senderName = value; OnPropertyChanged(); }
        }

        public string? Content
        {
            get => _content;
            set { _content = value; OnPropertyChanged(); }
        }

        public string ContentType
        {
            get => _contentType;
            set { _contentType = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsImage)); OnPropertyChanged(nameof(IsFile)); }
        }

        public string? FileUrl
        {
            get => _fileUrl;
            set { _fileUrl = value; OnPropertyChanged(); }
        }

        public string? FileName
        {
            get => _fileName;
            set { _fileName = value; OnPropertyChanged(); }
        }

        public bool IsRead
        {
            get => _isRead;
            set { _isRead = value; OnPropertyChanged(); }
        }

        public DateTime SentDate
        {
            get => _sentDate;
            set { _sentDate = value; OnPropertyChanged(); OnPropertyChanged(nameof(SentTimeFormatted)); }
        }

        // Alias property for backward compatibility (Spanish name)
        public DateTime FechaEnvio
        {
            get => SentDate;
            set => SentDate = value;
        }

        public bool EsPropio => IsFromUser;

        public string? Contenido
        {
            get => Content;
            set => Content = value;
        }

        public bool Leido
        {
            get => IsRead;
            set => IsRead = value;
        }

        // Propiedades calculadas
        public bool IsFromUser => SenderType == "USUARIO";
        public bool IsImage => ContentType == "IMAGEN";
        public bool IsFile => ContentType == "ARCHIVO";
        public string SentTimeFormatted => SentDate.ToString("HH:mm");

        // Para el layout del chat
        public LayoutOptions HorizontalAlignment => IsFromUser ? LayoutOptions.End : LayoutOptions.Start;
        public Color BubbleColor => IsFromUser ? Color.FromArgb("#DCF8C6") : Colors.White;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
