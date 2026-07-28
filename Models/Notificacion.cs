namespace FormularioMaquinaria.Models
{
    public class Notificacion
    {
        public int Id { get; set; }

        public string Titulo { get; set; } = string.Empty;

        public string Mensaje { get; set; } = string.Empty;

        public int ReporteMaquinariaId { get; set; }

        public bool Leida { get; set; } = false;

        public DateTime Fecha { get; set; } = DateTime.UtcNow;
    }
}