using System.ComponentModel.DataAnnotations;

namespace Maquinarias.Models
{
    public class ReporteMaquinaria
    {
        public int Id { get; set; }

        [Required]
        public string NombreOperador { get; set; }
        [Required]
        public string NombreMaquina { get; set; }

        [Required]
        public string TipoMaquina { get; set; }

        [Required]
        public decimal HorometroInicial { get; set; }

        [Required]
        public decimal HorometroFinal { get; set; }

        public string? Observaciones { get; set; }

        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        public string? FotoHorometroInicial { get; set; }

        public string? FotoHorometroFinal { get; set; }


        public int Horario { get; set; }

        public int ManejoMaquinaria { get; set; }

        public int CuidadoEquipo { get; set; }

        public int SeguridadIndustrial { get; set; }

        public int Productividad { get; set; }

        public int ReporteNovedades { get; set; }

        public string? ObservacionSupervisor { get; set; }

    }
}
