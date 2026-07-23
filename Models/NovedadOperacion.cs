using System.ComponentModel.DataAnnotations;

namespace FormularioMaquinaria.Models
{
    public class NovedadOperacion
    {
        public int Id { get; set; }

        [Required]
        public int ReporteMaquinariaId { get; set; }

        public ReporteMaquinaria Reporte { get; set; } = null!;

        [Required]
        public DateTime HoraInicio { get; set; } = DateTime.UtcNow;

        public DateTime? HoraFin { get; set; }

        [Required]
        [StringLength(100)]
        public string TipoNovedad { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Observacion { get; set; }

        public bool Activa { get; set; } = true;
    }
}