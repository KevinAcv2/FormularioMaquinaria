using FormularioMaquinaria.Models;

namespace FormularioMaquinaria.Models;

public class NovedadOperacion
{
    public int Id { get; set; }

    public int ReporteMaquinariaId { get; set; }
    public ReporteMaquinaria? Reporte { get; set; }

    public string TipoNovedad { get; set; } = string.Empty;

    public string Observacion { get; set; } = string.Empty;

    public DateTime HoraInicio { get; set; }

    public string? EvidenciaInicio { get; set; }

    public DateTime? HoraFin { get; set; }

    public string? ObservacionFin { get; set; }

    public string? EvidenciaFin { get; set; }

    public bool Activa { get; set; } = true;
}