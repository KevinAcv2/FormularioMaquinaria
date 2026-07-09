using Microsoft.AspNetCore.Http;

public class ReporteMaquinariaVM
{
    public string NombreOperador { get; set; }
    public string FrenteOperacional { get; set; }
    public string NombreMaquina { get; set; }

    public decimal HorometroInicial { get; set; }
    public decimal HorometroFinal { get; set; }

    public IFormFile FotoInicial { get; set; }
    public IFormFile FotoFinal { get; set; }

    public string? Observaciones { get; set; }
}