using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SentryHouseBackend.Models;

namespace SentryHouseBackend.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        protected AppDbContext() { }

        // Tablas (DbSets)
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<MateriaPrima> MateriasPrimas { get; set; }
        public DbSet<Cotizacion> Cotizaciones { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<Servicio> Servicios { get; set; }

        // Si manejas N:N entre Cotización y Servicios
        public DbSet<CotizacionServicio> CotizacionServicios { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configuración para relación N:N (Cotizacion <-> Servicio)
            builder.Entity<CotizacionServicio>()
                .HasKey(cs => new { cs.CotizacionId, cs.ServicioId });

            builder.Entity<CotizacionServicio>()
                .HasOne(cs => cs.Cotizacion)
                .WithMany(c => c.CotizacionServicios)
                .HasForeignKey(cs => cs.CotizacionId);

            builder.Entity<CotizacionServicio>()
                .HasOne(cs => cs.Servicio)
                .WithMany()
                .HasForeignKey(cs => cs.ServicioId);

        }
    }
}
