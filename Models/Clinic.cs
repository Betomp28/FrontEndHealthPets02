namespace FrontEndHealthPets.Models
{
    /// <summary>
    /// Model for veterinary clinic
    /// </summary>
    public class Clinic
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string VeterinarianName { get; set; } = "";
        public string Address { get; set; } = "";
        public string Phone { get; set; } = "";
        public string? Email { get; set; }
        public string? Description { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? PhotoUrl { get; set; }
        public bool IsOpen { get; set; }
        public string? OpeningHours { get; set; }

        // Spanish aliases
        public string Nombre { get => Name; set => Name = value; }
        public string Direccion { get => Address; set => Address = value; }
        public string Telefono { get => Phone; set => Phone = value; }
        public string NombreVeterinario { get => VeterinarianName; set => VeterinarianName = value; }
        public string? Descripcion { get => Description; set => Description = value; }
        public string? HorarioAtencion { get => OpeningHours; set => OpeningHours = value; }
    }
}
