using Maquinarias.Models;
using System;

namespace FormularioMaquinaria.Models
{
    public class ViajeMaterial
    {
        public int Id { get; set; }

        public string Recibidor { get; set; } = string.Empty;   

        public int FrenteOperacionalId { get; set; }

        public FrenteOperacional? FrenteOperacional { get; set; }

        public string Zona { get; set; } = string.Empty;

        public string Tramo { get; set; } = string.Empty;

        public string NumeroRecibo { get; set; } = string.Empty;

        public string Placa { get; set; } = string.Empty;

        public decimal VolumenM3 { get; set; }

        public string Material { get; set; } = string.Empty;

        public TimeOnly HoraSalida { get; set; }

        public TimeOnly HoraLlegada { get; set; }

        public string Origen { get; set; } = string.Empty;

        public string AbscisaDescargue { get; set; } = string.Empty;

        public string Destino { get; set; } = string.Empty;

        public string EvidenciaDescarga { get; set; } = string.Empty;

        public DateTime Fecha { get; set; } = DateTime.UtcNow;
    }
}
