using Maquinarias.Models;
using System.ComponentModel.DataAnnotations;

namespace FormularioMaquinaria.Models
{
    public class Operador
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        // FK
        public int? FrenteOperacionalId { get; set; }

        // Navegación
        public FrenteOperacional FrenteOperacional { get; set; } = null!;

        public int? MaquinaId { get; set; }

        public Maquina? Maquina { get; set; }
    }
}