using Maquinarias.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FormularioMaquinaria.Models;

namespace FormularioMaquinaria.ViewComponents;

public class AlertasViewComponent : ViewComponent
{
    private readonly AppDbContext _context;

    public AlertasViewComponent(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var alertas = await _context.NovedadesOperacion
            .Include(x => x.Reporte)
            .Where(x => x.Activa)
            .OrderByDescending(x => x.HoraInicio)
            .ToListAsync();

        return View(alertas);
    }
}