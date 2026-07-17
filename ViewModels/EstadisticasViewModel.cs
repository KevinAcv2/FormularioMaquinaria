namespace Maquinarias.ViewModels
{
    public class EstadisticasViewModel
    {
        public int TotalReportes { get; set; }

        public decimal TotalHoras { get; set; }

        public int TotalOperadores { get; set; }

        public int TotalMaquinas { get; set; }

        public List<string> Maquinas { get; set; } = new();
        public List<int> CantidadReportes { get; set; } = new();
        public List<string> Frente { get; set; } = new();
        public List<int> CantidadFrentes { get; set; } = new();
        public List<string> TopOperadores { get; set; } = new();
        public List<decimal> HorasOperadores { get; set; } = new();
        public List<string> Meses { get; set; } = new();
        public List<int> ReportePorMes { get; set; } = new();
    }
}