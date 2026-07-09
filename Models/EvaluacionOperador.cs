using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormularioMaquinaria.Models
{
    public class EvaluacionOperador
    {
        public int Id { get; set; }

        [Required]
        public int ReporteMaquinariaId { get; set; }

        [ForeignKey("ReporteMaquinariaId")]
        public ReporteMaquinaria? Reporte { get; set; }

        [Required]
        public int Horario { get; set; }

        [Required]
        public int ManejoMaquinaria { get; set; }

        [Required]
        public int CuidadoEquipo { get; set; }

        [Required]
        public int SeguridadIndustrial { get; set; }

        [Required]
        public int Productividad { get; set; }

        [Required]
        public int ReporteNovedades { get; set; }

        public string? ObservacionSupervisor { get; set; }

        public DateTime FechaEvaluacion { get; set; } = DateTime.UtcNow;
    }
}