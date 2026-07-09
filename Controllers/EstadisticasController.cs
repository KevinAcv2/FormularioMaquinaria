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

            var reportesMes = await _context.ReportesMaquinaria
                .GroupBy(r => new { r.Fecha.Year, r.Fecha.Month })
                .Select(g => new
                {
                    Mes = new DateTime(g.Key.Year, g.Key.Month, 1)
                        .ToString("MMM yyyy"),
                    Cantidad = g.Count(),
                    Orden = new DateTime(g.Key.Year, g.Key.Month, 1)
                })
                .OrderBy(x => x.Orden)
                .ToListAsync();

            modelo.Meses = reportesMes
                .Select(x => x.Mes)
                .ToList();

            modelo.ReportePorMes = reportesMes
                .Select(x => x.Cantidad)
                .ToList();

            return View(modelo);
        }
    }
}