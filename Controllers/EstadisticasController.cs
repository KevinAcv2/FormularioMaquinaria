using Maquinarias.Data;
using Maquinarias.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FormularioMaquinaria.Controllers
{
    public class EstadisticasController : Controller
    {
        private readonly AppDbContext _context;

        public EstadisticasController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var modelo = new EstadisticasViewModel
            {
                TotalReportes = await _context.ReportesMaquinaria.CountAsync(),

                TotalOperadores = await _context.Operadores.CountAsync(),

                TotalMaquinas = await _context.Maquinas.CountAsync(),

                TotalHoras = await _context.ReportesMaquinaria
                    .SumAsync(r => r.HorasTrabajadas)
            };

            var reportesPorMaquina = await _context.ReportesMaquinaria
                .GroupBy(r => r.NombreMaquina)
                .Select(g => new
                {
                    Maquina = g.Key,
                    CantidadReportes = g.Count()
                })
                .OrderByDescending(x => x.CantidadReportes)
                .ToListAsync();

            modelo.Maquinas = reportesPorMaquina
                .Select(x => x.Maquina)
                .ToList();

            modelo.CantidadReportes = reportesPorMaquina
                .Select(x => x.CantidadReportes)
                .ToList();

            var reportesPorFrente = await _context.ReportesMaquinaria
                .GroupBy(r => r.FrenteOperacional)
                .Select(g => new
                {
                    Frente = g.Key,
                    Cantidad = g.Count()
                })
                .ToListAsync();

            modelo.Frente = reportesPorFrente
                .Select(x => x.Frente)
                .ToList();

            modelo.CantidadFrentes = reportesPorFrente
                .Select(x => x.Cantidad)
                .ToList();

            var topOperadores = await _context.ReportesMaquinaria
                .GroupBy(r => r.NombreOperador)
                .Select(g => new
                {
                    Nombre = g.Key,
                    Horas = g.Sum(x => x.HorasTrabajadas)
                })
                .OrderByDescending(x => x.Horas)
                .Take(5)
                .ToListAsync();

            modelo.TopOperadores = topOperadores
                .Select(x => x.Nombre)
                .ToList();

            modelo.HorasOperadores = topOperadores
                .Select(x => x.Horas)
                .ToList();

            return View(modelo);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTendenciaSemanal(DateTime fecha)
        {
            fecha = DateTime.SpecifyKind(fecha, DateTimeKind.Utc);
            int diasDesdeLunes = fecha.DayOfWeek == DayOfWeek.Sunday
                ? 6
                : (int)fecha.DayOfWeek - 1;

            DateTime inicioSemana = fecha.Date.AddDays(-diasDesdeLunes);
            DateTime finSemana = inicioSemana.AddDays(7);

            var reportesSemana = await _context.ReportesMaquinaria
                .Where(r => r.Fecha >= inicioSemana && r.Fecha < finSemana)
                .ToListAsync();

            var etiquetas = new List<string>();
            var valores = new List<int>();

            for (int i = 0; i < 7; i++)
            {
                var dia = inicioSemana.AddDays(i).Date;

                etiquetas.Add(dia.ToString("ddd dd"));

                valores.Add(
                    reportesSemana.Count(r => r.Fecha.Date == dia)
                );
            }
            foreach (var r in reportesSemana)
            {
                Console.WriteLine($"Fecha BD: {r.Fecha:yyyy-MM-dd HH:mm:ss}");
            }

            Console.WriteLine($"Inicio: {inicioSemana:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"Fin: {finSemana:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"Registros encontrados: {reportesSemana.Count}");

            return Json(new
            {
                labels = etiquetas,
                data = valores
            });

        }
    }
}