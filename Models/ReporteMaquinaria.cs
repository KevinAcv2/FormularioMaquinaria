using System;

namespace FormularioMaquinaria.Models
{
    public class ReporteMaquinaria
    {
        public int Id { get; set; }

        public string NombreOperador { get; set; } = string.Empty;
        public string FrenteOperacional { get; set; } = string.Empty;
        public string NombreMaquina { get; set; } = string.Empty;
        public string TipoMaquina { get; set; } = string.Empty;

        public decimal HorometroInicial { get; set; }
        public decimal HorometroFinal { get; set; }
        public decimal HorasTrabajadas { get; set; }

        public int EstadoMaquina { get; set; }

        public string? Observaciones { get; set; }

        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public DateTime? FechaFin { get; set; }

        public string FotoHorometroInicial { get; set; } = string.Empty;
        public string FotoHorometroFinal { get; set; } = string.Empty;

        public EvaluacionOperador? Evaluacion { get; set; }

        public ICollection<NovedadOperacion> Novedades { get; set; } = new List<NovedadOperacion>();
    }
}