using System.Text.Json.Serialization;

namespace FrontEndHealthPets.Models
{
    public class Veterinarian
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        [JsonPropertyName("specialty")]
        public string? Specialty { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("profileImage")]
        public string? ProfileImage { get; set; }

        [JsonPropertyName("consultationPrice")]
        public decimal ConsultationPrice { get; set; }

        [JsonPropertyName("clinicName")]
        public string? ClinicName { get; set; }

        [JsonPropertyName("clinicAddress")]
        public string? ClinicAddress { get; set; }

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; } = true;

        [JsonPropertyName("rating")]
        public double Rating { get; set; }

        [JsonPropertyName("reviewCount")]
        public int ReviewCount { get; set; }

        [JsonPropertyName("yearsExperience")]
        public int YearsExperience { get; set; }

        // Computed properties for display
        public string RatingFormatted => $"{Rating:F1}";
        public string PriceFormatted => $"${ConsultationPrice:N0}";
        public string ExperienceFormatted => $"{YearsExperience} años de experiencia";
    }
}
