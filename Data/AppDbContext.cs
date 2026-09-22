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

        public DbSet<ViajeMaterial> ViajesMateriales { get; set; }
        public DbSet<Recibidor> Recibidores { get; set; }
        public DbSet<ReporteMaquinaria> ReportesMaquinaria { get; set; }
        public DbSet<Operador> Operadores { get; set; }
        public DbSet<Maquina> Maquinas { get; set; }
        public DbSet<EvaluacionOperador> EvaluacionesOperadores { get; set; }
        public DbSet<FrenteOperacional> FrentesOperacionales { get; set; }
        public DbSet<NovedadOperacion> NovedadesOperacion { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }
        public DbSet<Volqueta> Volquetas { get; set; }


        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            ConfigurarOperador(modelBuilder);
            ConfigurarEvaluacionOperador(modelBuilder);
            ConfigurarNovedadOperacion(modelBuilder);
            ConfigurarViajeMaterial(modelBuilder);
            ConfigurarDatosIniciales(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private static void ConfigurarOperador(
            ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Operador>()
                .HasOne(o => o.Maquina)
                .WithMany()
                .HasForeignKey(o => o.MaquinaId)
                .OnDelete(DeleteBehavior.SetNull);


            modelBuilder.Entity<Operador>()
                .HasOne(o => o.FrenteOperacional)
                .WithMany(f => f.Operadores)
                .HasForeignKey(o => o.FrenteOperacionalId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private static void ConfigurarEvaluacionOperador(
            ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EvaluacionOperador>()
                .HasOne(e => e.Reporte)
                .WithOne(r => r.Evaluacion)
                .HasForeignKey<EvaluacionOperador>(
                    e => e.ReporteMaquinariaId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private static void ConfigurarNovedadOperacion(
            ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NovedadOperacion>()
                .HasOne(n => n.Reporte)
                .WithMany(r => r.Novedades)
                .HasForeignKey(n => n.ReporteMaquinariaId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private static void ConfigurarViajeMaterial(
            ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ViajeMaterial>()
                .HasOne(v => v.FrenteOperacional)
                .WithMany()
                .HasForeignKey(v => v.FrenteOperacionalId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<ViajeMaterial>()
                .HasOne(v => v.Recibidor)
                .WithMany(r => r.ViajesMaterial)
                .HasForeignKey(v => v.RecibidorId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<ViajeMaterial>()
                .Property(v => v.VolumenM3)
                .HasColumnType("numeric(18,2)");


            modelBuilder.Entity<ViajeMaterial>()
                .Property(v => v.Fecha)
                .HasColumnType("timestamp with time zone");
        }


        private static void ConfigurarDatosIniciales(
            ModelBuilder modelBuilder)
        {
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
        }
    }
}