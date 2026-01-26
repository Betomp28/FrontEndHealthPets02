using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FrontEndHealthPets.Models
{
    /// <summary>
    /// Notificación del sistema
    /// </summary>
    public class Notificacion : INotifyPropertyChanged
    {
        private int _id;
        private string _type = string.Empty;
        private string _title = string.Empty;
        private string _message = string.Empty;
        private string? _icon;
        private bool _isRead;
        private DateTime? _sentDate;
        private DateTime _createdAt;
        private string? _entityType;
        private int? _entityId;

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        public string Type
        {
            get => _type;
            set { _type = value; OnPropertyChanged(); OnPropertyChanged(nameof(TypeIcon)); }
        }

        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(); }
        }

        public string Message
        {
            get => _message;
            set { _message = value; OnPropertyChanged(); }
        }

        public string? Icon
        {
            get => _icon;
            set { _icon = value; OnPropertyChanged(); }
        }

        public bool IsRead
        {
            get => _isRead;
            set { _isRead = value; OnPropertyChanged(); OnPropertyChanged(nameof(BackgroundColor)); }
        }

        public DateTime? SentDate
        {
            get => _sentDate;
            set { _sentDate = value; OnPropertyChanged(); }
        }

        public DateTime CreatedAt
        {
            get => _createdAt;
            set { _createdAt = value; OnPropertyChanged(); OnPropertyChanged(nameof(TimeAgo)); }
        }

        public string? EntityType
        {
            get => _entityType;
            set { _entityType = value; OnPropertyChanged(); }
        }

        public int? EntityId
        {
            get => _entityId;
            set { _entityId = value; OnPropertyChanged(); }
        }

        // Alias properties for backward compatibility (Spanish names)
        public DateTime FechaCreacion
        {
            get => CreatedAt;
            set => CreatedAt = value;
        }

        public bool Leida
        {
            get => IsRead;
            set => IsRead = value;
        }

        public string Tipo
        {
            get => Type;
            set => Type = value;
        }

        public string Titulo
        {
            get => Title;
            set => Title = value;
        }

        public string Mensaje
        {
            get => Message;
            set => Message = value;
        }

        // Propiedades calculadas
        public string TypeIcon => Type switch
        {
            "VACUNA" => "vaccine_icon.png",
            "MEDICAMENTO" => "medicine_icon.png",
            "CITA" => "calendar_icon.png",
            "CONSULTA" => "stethoscope_icon.png",
            "MENSAJE" => "chat_icon.png",
            "RESENA" => "star_icon.png",
            _ => "notification_icon.png"
        };

        public Color BackgroundColor => IsRead ? Colors.White : Color.FromArgb("#E3F2FD");

        public string TimeAgo
        {
            get
            {
                var diff = DateTime.UtcNow - CreatedAt;
                if (diff.TotalMinutes < 1) return "Ahora";
                if (diff.TotalMinutes < 60) return $"Hace {(int)diff.TotalMinutes} min";
                if (diff.TotalHours < 24) return $"Hace {(int)diff.TotalHours} h";
                if (diff.TotalDays < 7) return $"Hace {(int)diff.TotalDays} días";
                return CreatedAt.ToString("dd/MM/yyyy");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    /// <summary>
    /// Token de dispositivo para push notifications
    /// </summary>
    public class DeviceToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;
        public string? DeviceModel { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Recordatorio programado
    /// </summary>
    public class Recordatorio : INotifyPropertyChanged
    {
        private int _id;
        private int? _petId;
        private string? _petName;
        private string _type = string.Empty;
        private string _title = string.Empty;
        private string _message = string.Empty;
        private DateTime _reminderDate;
        private bool _isRecurring;
        private int? _frequencyDays;
        private bool _isActive;
        private bool _isSent;

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
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

        public string Type
        {
            get => _type;
            set { _type = value; OnPropertyChanged(); }
        }

        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(); }
        }

        public string Message
        {
            get => _message;
            set { _message = value; OnPropertyChanged(); }
        }

        public DateTime ReminderDate
        {
            get => _reminderDate;
            set { _reminderDate = value; OnPropertyChanged(); OnPropertyChanged(nameof(ReminderDateFormatted)); }
        }

        public bool IsRecurring
        {
            get => _isRecurring;
            set { _isRecurring = value; OnPropertyChanged(); }
        }

        public int? FrequencyDays
        {
            get => _frequencyDays;
            set { _frequencyDays = value; OnPropertyChanged(); }
        }

        public bool IsActive
        {
            get => _isActive;
            set { _isActive = value; OnPropertyChanged(); }
        }

        public bool IsSent
        {
            get => _isSent;
            set { _isSent = value; OnPropertyChanged(); }
        }

        // Alias property for backward compatibility (Spanish name)
        public DateTime FechaProgramada
        {
            get => ReminderDate;
            set => ReminderDate = value;
        }

        public string ReminderDateFormatted => ReminderDate.ToString("dd/MM/yyyy HH:mm");

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
