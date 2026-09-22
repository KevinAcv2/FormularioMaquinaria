using System.ComponentModel.DataAnnotations;

namespace FormularioMaquinaria.Models
{
    public class Volqueta
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Placa { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Propiedad { get; set; } = string.Empty;

        [StringLength(150)]
        public string? EmpresaPropietaria { get; set; }

        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "1";

        [StringLength(500)]
        public string? Observaciones { get; set; }
    }
}