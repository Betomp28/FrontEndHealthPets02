namespace FrontEndHealthPets.Models
{
    /// <summary>
    /// Represents a veterinary clinic
    /// </summary>
    public class Veterinary
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public List<string> Especialidades { get; set; } = new();
        public string HorarioAtencion { get; set; } = string.Empty;
        public bool IsOpen { get; set; }
        public bool IsEmergency24h { get; set; }
        public double DistanceKm { get; set; }

        // Display helpers
        public string RatingDisplay => $"⭐ {Rating:F1} ({ReviewCount})";
        public string DistanceDisplay => DistanceKm < 1 
            ? $"{DistanceKm * 1000:F0}m" 
            : $"{DistanceKm:F1}km";
        public string StatusDisplay => IsOpen ? "Abierto" : "Cerrado";
        public Color StatusColor => IsOpen ? Colors.Green : Colors.Red;
        public string EmergencyBadge => IsEmergency24h ? "🚨 24h" : "";
    }
}
