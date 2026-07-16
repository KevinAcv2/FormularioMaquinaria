using FormularioMaquinaria.Models;
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
        public DbSet<Operador> Operadores { get; set; }

        public DbSet<Maquina> Maquinas { get; set; }

        public DbSet<EvaluacionOperador> EvaluacionesOperadores { get; set; }

        public DbSet<FrenteOperacional> FrentesOperacionales { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relación Operador -> Máquina
            modelBuilder.Entity<Operador>()
                .HasOne(o => o.Maquina)
                .WithMany()
                .HasForeignKey(o => o.MaquinaId)
                .OnDelete(DeleteBehavior.SetNull);


            // Relación Operador -> Frente Operacional
            modelBuilder.Entity<Operador>()
                .HasOne(o => o.FrenteOperacional)
                .WithMany(f => f.Operadores)
                .HasForeignKey(o => o.FrenteOperacionalId)
                .OnDelete(DeleteBehavior.Restrict);


            // Relación Evaluación -> Reporte
            modelBuilder.Entity<EvaluacionOperador>()
                .HasOne(e => e.Reporte)
                .WithOne(r => r.Evaluacion)
                .HasForeignKey<EvaluacionOperador>(e => e.ReporteMaquinariaId)
                .OnDelete(DeleteBehavior.Cascade);


            // Datos iniciales de Frentes Operacionales
            modelBuilder.Entity<FrenteOperacional>().HasData(

                new FrenteOperacional
                {
                    Id = 1,
                    Nombre = "FRENTE PADEL"
                },

                new FrenteOperacional
                {
                    Id = 2,
                    Nombre = "FRENTE PANAMÁ"
                },

                new FrenteOperacional
                {
                    Id = 3,
                    Nombre = "CANTERA RIO SECO"
                }

            );


            base.OnModelCreating(modelBuilder);
        }

    }
}
