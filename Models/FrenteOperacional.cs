using FormularioMaquinaria.Models;
using System.ComponentModel.DataAnnotations;

namespace Maquinarias.Models
{
    public class FrenteOperacional
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;


        // Relación con operadores
        public ICollection<Operador>? Operadores { get; set; }
    }
}