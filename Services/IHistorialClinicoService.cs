using FrontEndHealthPets.Models;

namespace FrontEndHealthPets.Services
{
    /// <summary>
    /// Interfaz para el servicio de historial clínico
    /// </summary>
    public interface IHistorialClinicoService
    {
        /// <summary>
        /// Obtiene el historial clínico completo de una mascota
        /// </summary>
        Task<HistorialClinico?> GetHistorialClinicoAsync(long petId);

        /// <summary>
        /// Obtiene las consultas de una mascota
        /// </summary>
        Task<List<Consulta>> GetConsultasByPetAsync(long petId);

        /// <summary>
        /// Obtiene una consulta específica
        /// </summary>
        Task<Consulta?> GetConsultaAsync(int id);

        /// <summary>
        /// Crea una nueva consulta (solo veterinarios)
        /// </summary>
        Task<(bool success, Consulta? consulta, string error)> CreateConsultaAsync(ConsultaCreateRequest request);

        /// <summary>
        /// Actualiza una consulta
        /// </summary>
        Task<(bool success, string error)> UpdateConsultaAsync(int id, ConsultaCreateRequest request);

        /// <summary>
        /// Obtiene los archivos de una consulta
        /// </summary>
        Task<List<ConsultaArchivo>> GetArchivosAsync(int consultaId);

        /// <summary>
        /// Sube un archivo a una consulta
        /// </summary>
        Task<(bool success, ConsultaArchivo? archivo, string error)> UploadArchivoAsync(
            int consultaId, Stream fileStream, string fileName, string fileType, string? description);

        /// <summary>
        /// Elimina un archivo de una consulta
        /// </summary>
        Task<bool> DeleteArchivoAsync(int archivoId);

        /// <summary>
        /// Obtiene las consultas del veterinario actual
        /// </summary>
        Task<List<Consulta>> GetVetConsultasAsync(DateTime? from = null, DateTime? to = null);
    }
}
