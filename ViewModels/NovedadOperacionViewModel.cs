using System.ComponentModel.DataAnnotations;

namespace Maquinarias.ViewModels
{
    public class NovedadOperacionViewModel
    {
        [Required]
        public int ReporteMaquinariaId { get; set; }

        [Required(ErrorMessage = "Seleccione un tipo de novedad.")]
        public string TipoNovedad { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Observacion { get; set; }
        public IFormFile? EvidenciaInicio { get; set; }
    }
}