using System.ComponentModel.DataAnnotations;

namespace FormularioMaquinaria.Models
{
    public class Operador
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string FrenteOperacional { get; set; }

        public int? MaquinaId { get; set; }

        public Maquina? Maquina { get; set; }
    }
}