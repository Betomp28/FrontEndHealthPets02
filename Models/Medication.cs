using System;

namespace FrontEndHealthPets.Models
{
    /// <summary>
    /// Estado del tratamiento de medicamento
    /// </summary>
    public enum MedicationStatus
    {
        Active,      // En Tratamiento (Verde)
        Finished     // Finalizado (Gris)
    }

    /// <summary>
    /// Información simplificada del veterinario que recetó el medicamento
    /// </summary>
    public class PrescribingVet
    {
        public string Id { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Clinica { get; set; } = string.Empty;
    }

    /// <summary>
    /// Modelo de medicamento/receta médica
    /// </summary>
    public class Medication
    {
        public string MedicamentoId { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Dosis { get; set; } = string.Empty;
        public string Frecuencia { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public long MascotaId { get; set; }
        public PrescribingVet Veterinario { get; set; } = new PrescribingVet();
        public string InstruccionesAdicionales { get; set; } = string.Empty;

        /// <summary>
        /// Calcula dinámicamente el estado del tratamiento basándose en las fechas
        /// </summary>
        public MedicationStatus Status
        {
            get
            {
                var today = DateTime.Today;

                // Si la fecha actual está dentro del rango de tratamiento
                if (today >= FechaInicio && today <= FechaFin)
                {
                    return MedicationStatus.Active;
                }

                // Si ya terminó (fecha actual > fecha fin)
                return MedicationStatus.Finished;
            }
        }

        /// <summary>
        /// Texto formateado de dosis e instrucciones para mostrar en la UI
        /// </summary>
        public string DosisFormateada => $"{Dosis} - {Frecuencia}";

        /// <summary>
        /// Días restantes de tratamiento (si está activo)
        /// </summary>
        public int DiasRestantes
        {
            get
            {
                if (Status == MedicationStatus.Finished) return 0;
                return (FechaFin - DateTime.Today).Days;
            }
        }
    }
}
