using System.ComponentModel.DataAnnotations;

namespace FormularioMaquinaria.Models
{
    public class Recibidor
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del recibidor es obligatorio.")]
        [StringLength(150)]
        public string Nombre { get; set; } = string.Empty;

        public bool Habilitado { get; set; } = true;

        public ICollection<ViajeMaterial> ViajesMaterial { get; set; }
            = new List<ViajeMaterial>();
    }
}