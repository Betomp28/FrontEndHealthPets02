namespace FrontEndHealthPets.Models
{
    /// <summary>
    /// User model
    /// </summary>
    public class User
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }


        public DateTime CreatedAt { get; set; }
        
        public string FullName => $"{Nombre} {Apellidos}";
    }
}
