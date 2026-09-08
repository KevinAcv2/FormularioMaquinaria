using Maquinarias.Models;
using System.ComponentModel.DataAnnotations;

namespace FormularioMaquinaria.Models
{
    public class ViajeMaterial
    {
        public int Id { get; set; }
        public int RecibidorId { get; set; }
        public Recibidor? Recibidor { get; set; }
        public int FrenteOperacionalId { get; set; }
        public FrenteOperacional? FrenteOperacional { get; set; }
        public string Zona { get; set; } = string.Empty;
        public string Tramo { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d+$",
            ErrorMessage = "El número de recibo solo puede contener números.")]
        public string NumeroRecibo { get; set; } = string.Empty;


        private string _placa = string.Empty;

        [Required]
        public string Placa
        {
            get => _placa;

            set => _placa = string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : value.Trim().ToUpperInvariant();
        }
        public decimal VolumenM3 { get; set; }
        public string Material { get; set; } = string.Empty;
        public TimeOnly HoraLlegada { get; set; }
        public string Origen { get; set; } = string.Empty;

        public string AbscisaDescargue { get; set; } = string.Empty;

        public string Destino { get; set; } = string.Empty;
        public string EvidenciaDescarga { get; set; } = string.Empty;
        public DateTime Fecha { get; private set; } = DateTime.UtcNow;
        public void EstablecerFechaRegistro()
        {
            Fecha = DateTime.UtcNow;
        }
    }
}