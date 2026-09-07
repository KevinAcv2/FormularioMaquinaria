using FormularioMaquinaria.Models;
using Maquinarias.Models;
using Microsoft.EntityFrameworkCore;

namespace Maquinarias.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(
            DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }


        // =====================================================
        // DBSETS
        // =====================================================

        public DbSet<ViajeMaterial> ViajesMateriales { get; set; }

        public DbSet<ReporteMaquinaria> ReportesMaquinaria { get; set; }

        public DbSet<Operador> Operadores { get; set; }

        public DbSet<Maquina> Maquinas { get; set; }

        public DbSet<EvaluacionOperador> EvaluacionesOperadores { get; set; }

        public DbSet<FrenteOperacional> FrentesOperacionales { get; set; }

        public DbSet<NovedadOperacion> NovedadesOperacion { get; set; }

        public DbSet<Notificacion> Notificaciones { get; set; }


        // =====================================================
        // CONFIGURACIÓN DE MODELOS
        // =====================================================

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            // =================================================
            // RELACIÓN OPERADOR -> MÁQUINA
            // =================================================

            modelBuilder.Entity<Operador>()
                .HasOne(o => o.Maquina)
                .WithMany()
                .HasForeignKey(o => o.MaquinaId)
                .OnDelete(DeleteBehavior.SetNull);


            // =================================================
            // RELACIÓN OPERADOR -> FRENTE OPERACIONAL
            // =================================================

            modelBuilder.Entity<Operador>()
                .HasOne(o => o.FrenteOperacional)
                .WithMany(f => f.Operadores)
                .HasForeignKey(o => o.FrenteOperacionalId)
                .OnDelete(DeleteBehavior.Restrict);


            // =================================================
            // RELACIÓN EVALUACIÓN -> REPORTE
            // =================================================

            modelBuilder.Entity<EvaluacionOperador>()
                .HasOne(e => e.Reporte)
                .WithOne(r => r.Evaluacion)
                .HasForeignKey<EvaluacionOperador>(
                    e => e.ReporteMaquinariaId)
                .OnDelete(DeleteBehavior.Cascade);


            // =================================================
            // RELACIÓN REPORTE -> NOVEDADES
            // =================================================

            modelBuilder.Entity<NovedadOperacion>()
                .HasOne(n => n.Reporte)
                .WithMany(r => r.Novedades)
                .HasForeignKey(n => n.ReporteMaquinariaId)
                .OnDelete(DeleteBehavior.Cascade);


            // =================================================
            // RELACIÓN VIAJE MATERIAL -> FRENTE OPERACIONAL
            // =================================================

            modelBuilder.Entity<ViajeMaterial>()
                .HasOne(v => v.FrenteOperacional)
                .WithMany()
                .HasForeignKey(v => v.FrenteOperacionalId)
                .OnDelete(DeleteBehavior.Restrict);


            // =================================================
            // CONFIGURACIÓN DE VIAJE MATERIAL
            // =================================================

            modelBuilder.Entity<ViajeMaterial>()
                .Property(v => v.VolumenM3)
                .HasColumnType("numeric(18,2)");


            modelBuilder.Entity<ViajeMaterial>()
                .Property(v => v.Fecha)
                .HasColumnType("timestamp with time zone");


            // =================================================
            // DATOS INICIALES DE FRENTES OPERACIONALES
            // =================================================

            modelBuilder.Entity<FrenteOperacional>()
                .HasData(

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


            // =================================================
            // BASE
            // =================================================

            base.OnModelCreating(modelBuilder);
        }
    }
}