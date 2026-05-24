using Maquinarias.Models;
using Microsoft.EntityFrameworkCore;

namespace Maquinarias.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<ReporteMaquinaria> ReportesMaquinaria { get; set; }
    }
}
