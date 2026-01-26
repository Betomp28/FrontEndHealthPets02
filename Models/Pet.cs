using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FrontEndHealthPets.Models
{
    public class Pet : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private long _id;
        public long Id 
        { 
            get => _id; 
            set { if (_id != value) { _id = value; OnPropertyChanged(); } } 
        }

        private string _name = string.Empty;
        public string Name 
        { 
            get => _name; 
            set { if (_name != value) { _name = value; OnPropertyChanged(); } } 
        }

        private string _species = string.Empty;
        public string Species 
        { 
            get => _species; 
            set { if (_species != value) { _species = value; OnPropertyChanged(); } } 
        }

        private string _breed = string.Empty;
        public string Breed 
        { 
            get => _breed; 
            set { if (_breed != value) { _breed = value; OnPropertyChanged(); } } 
        }

        private DateTime _birthDate;
        public DateTime BirthDate 
        { 
            get => _birthDate; 
            set { if (_birthDate != value) { _birthDate = value; OnPropertyChanged(); OnPropertyChanged(nameof(AgeDisplay)); OnPropertyChanged(nameof(AgeInYears)); OnPropertyChanged(nameof(AgeInMonths)); OnPropertyChanged(nameof(LifeStage)); } } 
        }

        private double _weight;
        public double Weight 
        { 
            get => _weight; 
            set { if (_weight != value) { _weight = value; OnPropertyChanged(); OnPropertyChanged(nameof(WeightDisplay)); } } 
        }

        private string _color = string.Empty;
        public string Color 
        { 
            get => _color; 
            set { if (_color != value) { _color = value; OnPropertyChanged(); } } 
        }

        private string _gender = string.Empty;
        public string Gender 
        { 
            get => _gender; 
            set { if (_gender != value) { _gender = value; OnPropertyChanged(); } } 
        }

        private string _photoUrl = string.Empty;
        public string PhotoUrl 
        { 
            get => _photoUrl; 
            set 
            { 
                if (_photoUrl != value) 
                { 
                    _photoUrl = value; 
                    OnPropertyChanged(); 
                    OnPropertyChanged(nameof(ProfileImage)); 
                } 
            } 
        }

        private string _microchipNumber = string.Empty;
        public string MicrochipNumber 
        { 
            get => _microchipNumber; 
            set { if (_microchipNumber != value) { _microchipNumber = value; OnPropertyChanged(); } } 
        }

        private string _notes = string.Empty;
        public string Notes 
        { 
            get => _notes; 
            set { if (_notes != value) { _notes = value; OnPropertyChanged(); } } 
        }

        private string _cacheBuster = string.Empty;
        public string CacheBuster
        {
            get => _cacheBuster;
            set { if (_cacheBuster != value) { _cacheBuster = value; OnPropertyChanged(); OnPropertyChanged(nameof(ProfileImage)); } }
        }

        private double _photoOffsetY;
        public double PhotoOffsetY
        {
            get => _photoOffsetY;
            set { if (_photoOffsetY != value) { _photoOffsetY = value; OnPropertyChanged(); } }
        }

        public long UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Computed properties
        public string AgeDisplay
        {
            get
            {
                var age = DateTime.Now - BirthDate;
                var years = (int)(age.TotalDays / 365.25);
                var months = (int)((age.TotalDays % 365.25) / 30.44);

                if (years >= 1)
                {
                    if (months > 0)
                        return $"{years} año{(years > 1 ? "s" : "")} y {months} mes{(months > 1 ? "es" : "")}";
                    return $"{years} año{(years > 1 ? "s" : "")}";
                }
                else if (months >= 1)
                {
                    return $"{months} mes{(months > 1 ? "es" : "")}";
                }
                else
                {
                    var days = (int)age.TotalDays;
                    return $"{days} día{(days > 1 ? "s" : "")}";
                }
            }
        }

        public string WeightDisplay => $"{Weight:F1} kg";

        public string ProfileImage 
        {
            get
            {
                if (string.IsNullOrEmpty(PhotoUrl)) return "default_pet.png";
                if (string.IsNullOrEmpty(CacheBuster)) return PhotoUrl;
                
                // Append cache buster to URL
                string separator = PhotoUrl.Contains("?") ? "&" : "?";
                return $"{PhotoUrl}{separator}cb={CacheBuster}";
            }
        }

        public int AgeInYears => (int)((DateTime.Now - BirthDate).TotalDays / 365.25);

        public int AgeInMonths => (int)((DateTime.Now - BirthDate).TotalDays / 30.44);

        public bool IsAdult => AgeInYears >= 1;

        public bool IsSenior => AgeInYears >= 7; // Consider senior at 7 years

        public string LifeStage
        {
            get
            {
                if (AgeInMonths < 6) return "Cachorro";
                if (AgeInMonths < 12) return "Joven";
                if (AgeInYears < 7) return "Adulto";
                return "Senior";
            }
        }

        // For vaccine status display
        public string VaccineStatus { get; set; } = "Al día";
        public string VaccineStatusColor { get; set; } = "#00D9A5";

        // For appointments
        public string NextAppointment { get; set; } = "Próx. cita";
        public string NextAppointmentColor { get; set; } = "#FFB800";
    }
}
