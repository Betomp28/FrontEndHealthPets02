namespace FrontEndHealthPets.Models
{
    /// <summary>
    /// Veterinarian profile model for vet portal
    /// </summary>
    public class VetProfile
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Specialty { get; set; }
        public string? Description { get; set; }
        public string? ProfileImage { get; set; }
        public decimal ConsultationPrice { get; set; }
        public string? ClinicName { get; set; }
        public int TodayAppointments { get; set; }
        public int PendingAppointments { get; set; }
        public string? Token { get; set; }

        public string DisplayName => $"Dr. {Name}";
        public string PriceFormatted => $"₡{ConsultationPrice:N0}";
    }
}
