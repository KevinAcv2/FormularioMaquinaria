using FormularioMaquinaria.Models;
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
        public DbSet<Operador> Operadores { get; set; }

        public DbSet<Maquina> Maquinas { get; set; }

        public DbSet<EvaluacionOperador> EvaluacionesOperadores { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Operador>()
                .HasOne(o => o.Maquina)
                .WithMany()
                .HasForeignKey(o => o.MaquinaId)
                .OnDelete(DeleteBehavior.SetNull);

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EvaluacionOperador>()
                .HasOne(e => e.Reporte)
                .WithOne(r => r.Evaluacion)
                .HasForeignKey<EvaluacionOperador>(e => e.ReporteMaquinariaId)
                .OnDelete(DeleteBehavior.Cascade);

        }

    }
}
