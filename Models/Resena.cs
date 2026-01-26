using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FrontEndHealthPets.Models
{
    /// <summary>
    /// Reseña/calificación de un veterinario
    /// </summary>
    public class Resena : INotifyPropertyChanged
    {
        private int _id;
        private int _userId;
        private string _userName = string.Empty;
        private int _veterinarianId;
        private string _veterinarianName = string.Empty;
        private int _rating;
        private string? _comment;
        private int? _punctualityRating;
        private int? _attentionRating;
        private int? _explanationRating;
        private int? _facilitiesRating;
        private bool _isVerified;
        private string? _vetResponse;
        private DateTime? _vetResponseDate;
        private DateTime _reviewDate;

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

        public int Rating
        {
            get => _rating;
            set { _rating = value; OnPropertyChanged(); OnPropertyChanged(nameof(StarsDisplay)); }
        }

        public string? Comment
        {
            get => _comment;
            set { _comment = value; OnPropertyChanged(); }
        }

        public int? PunctualityRating
        {
            get => _punctualityRating;
            set { _punctualityRating = value; OnPropertyChanged(); }
        }

        public int? AttentionRating
        {
            get => _attentionRating;
            set { _attentionRating = value; OnPropertyChanged(); }
        }

        public int? ExplanationRating
        {
            get => _explanationRating;
            set { _explanationRating = value; OnPropertyChanged(); }
        }

        public int? FacilitiesRating
        {
            get => _facilitiesRating;
            set { _facilitiesRating = value; OnPropertyChanged(); }
        }

        public bool IsVerified
        {
            get => _isVerified;
            set { _isVerified = value; OnPropertyChanged(); }
        }

        public string? VetResponse
        {
            get => _vetResponse;
            set { _vetResponse = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasVetResponse)); }
        }

        public DateTime? VetResponseDate
        {
            get => _vetResponseDate;
            set { _vetResponseDate = value; OnPropertyChanged(); }
        }

        public DateTime ReviewDate
        {
            get => _reviewDate;
            set { _reviewDate = value; OnPropertyChanged(); OnPropertyChanged(nameof(ReviewDateFormatted)); }
        }

        // Alias properties for backward compatibility (Spanish names)
        public DateTime FechaCreacion
        {
            get => ReviewDate;
            set => ReviewDate = value;
        }

        public string VeterinarioNombre
        {
            get => VeterinarianName;
            set => VeterinarianName = value;
        }

        public int Calificacion
        {
            get => Rating;
            set => Rating = value;
        }

        public string? Comentario
        {
            get => Comment;
            set => Comment = value;
        }

        public string? RespuestaVeterinario
        {
            get => VetResponse;
            set => VetResponse = value;
        }

        public int VeterinarioId
        {
            get => VeterinarianId;
            set => VeterinarianId = value;
        }

        // Propiedades calculadas
        public string StarsDisplay => new string('★', Rating) + new string('☆', 5 - Rating);
        public bool HasVetResponse => !string.IsNullOrEmpty(VetResponse);
        public string ReviewDateFormatted => ReviewDate.ToString("dd/MM/yyyy");

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    /// <summary>
    /// Resumen de calificaciones de un veterinario
    /// </summary>
    public class VeterinarianRatingSummary
    {
        public int VeterinarianId { get; set; }
        public string VeterinarianName { get; set; } = string.Empty;
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public int FiveStarCount { get; set; }
        public int FourStarCount { get; set; }
        public int ThreeStarCount { get; set; }
        public int TwoStarCount { get; set; }
        public int OneStarCount { get; set; }
        public decimal? AveragePunctuality { get; set; }
        public decimal? AverageAttention { get; set; }
        public decimal? AverageExplanation { get; set; }
        public decimal? AverageFacilities { get; set; }

        // Propiedades calculadas
        public string AverageRatingFormatted => AverageRating.ToString("F1");
        public string StarsDisplay => new string('★', (int)Math.Round(AverageRating)) + new string('☆', 5 - (int)Math.Round(AverageRating));

        public double FiveStarPercentage => TotalReviews > 0 ? (double)FiveStarCount / TotalReviews * 100 : 0;
        public double FourStarPercentage => TotalReviews > 0 ? (double)FourStarCount / TotalReviews * 100 : 0;
        public double ThreeStarPercentage => TotalReviews > 0 ? (double)ThreeStarCount / TotalReviews * 100 : 0;
        public double TwoStarPercentage => TotalReviews > 0 ? (double)TwoStarCount / TotalReviews * 100 : 0;
        public double OneStarPercentage => TotalReviews > 0 ? (double)OneStarCount / TotalReviews * 100 : 0;
    }
}
