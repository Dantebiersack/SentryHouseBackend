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
        public DbSet<CotizacionServicio> CotizacionServicios { get; set; }

        public DbSet<CompraProveedor> ComprasProveedores { get; set; }
        public DbSet<CompraDetalle> ComprasDetalles { get; set; }

        // Nuevos para Ventas/BOM
        public DbSet<ServicioMaterial> ServiciosMateriales { get; set; }
        public DbSet<VentaDetalle> VentasDetalles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ----- COTIZACIONES <-> SERVICIOS (N:N) -----
            builder.Entity<CotizacionServicio>()
                .HasKey(cs => new { cs.CotizacionId, cs.ServicioId });

            builder.Entity<CotizacionServicio>()
                .HasOne(cs => cs.Cotizacion)
                .WithMany(c => c.CotizacionServicios)
                .HasForeignKey(cs => cs.CotizacionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CotizacionServicio>()
                .HasOne(cs => cs.Servicio)
                .WithMany() // no colección inversa en Servicio
                .HasForeignKey(cs => cs.ServicioId)
                .OnDelete(DeleteBehavior.Restrict);

            // ----- VENTA ↔ USUARIO (AspNetUsers) -----
            builder.Entity<Venta>()
                .HasOne(v => v.Usuario)
                .WithMany(u => u.Ventas)
                .HasForeignKey(v => v.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // ----- COTIZACION ↔ VENTA (1:1) -----
            // Según tu modelo actual, Cotizacion tiene VentaGenerada y Venta tiene CotizacionId
            builder.Entity<Venta>()
                .HasOne(v => v.Cotizacion)
                .WithOne(c => c.VentaGenerada)
                .HasForeignKey<Venta>(v => v.CotizacionId)
                .OnDelete(DeleteBehavior.Restrict);

            // ----- COMPRAS -----
            builder.Entity<CompraProveedor>()
                .HasOne(c => c.Proveedor)
                .WithMany() // sin colección inversa → evita ciclos
                .HasForeignKey(c => c.ProveedorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CompraDetalle>()
                .HasOne(d => d.Compra)
                .WithMany(c => c.Detalles)
                .HasForeignKey(d => d.CompraProveedorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CompraDetalle>()
                .HasOne(d => d.MateriaPrima)
                .WithMany() // sin colección inversa
                .HasForeignKey(d => d.MateriaPrimaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CompraProveedor>()
                .HasIndex(c => new { c.ProveedorId, c.Fecha, c.Estado });

            // ----- MATERIA PRIMA ↔ PROVEEDOR (1:N) -----
            // La usas en GET MateriaPrima con Include(Provedor), dejamos Restrict para evitar borrados accidentales.
            builder.Entity<MateriaPrima>()
                .HasOne(mp => mp.Proveedor)
                .WithMany(p => p.MateriasPrimas)
                .HasForeignKey(mp => mp.ProveedorId)
                .OnDelete(DeleteBehavior.Restrict);

            // ----- SERVICIO ↔ MATERIA PRIMA (BOM) -----
            builder.Entity<ServicioMaterial>()
                .HasOne(sm => sm.Servicio)
                .WithMany(s => s.Materiales) // si tu clase Servicio no tiene la colección, cambia a .WithMany()
                .HasForeignKey(sm => sm.ServicioId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ServicioMaterial>()
                .HasOne(sm => sm.MateriaPrima)
                .WithMany() // no colección inversa en MateriaPrima → sin ciclos
                .HasForeignKey(sm => sm.MateriaPrimaId)
                .OnDelete(DeleteBehavior.Restrict);

            // ----- VENTA ↔ DETALLE (1:N) y DETALLE ↔ SERVICIO (N:1) -----
            builder.Entity<VentaDetalle>()
                .HasOne(vd => vd.Venta)
                .WithMany(v => v.Detalles)
                .HasForeignKey(vd => vd.VentaId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<VentaDetalle>()
                .HasOne(vd => vd.Servicio)
                .WithMany() // no colección inversa
                .HasForeignKey(vd => vd.ServicioId)
                .OnDelete(DeleteBehavior.Restrict);

            // ----- Índices útiles para Ventas -----
            builder.Entity<Venta>()
                .HasIndex(v => new { v.UsuarioId, v.FechaVenta });

            // Si agregaste EstadoVenta en Venta, puedes indexarlo:
             builder.Entity<Venta>().HasIndex(v => v.Estado);

            // ----- COTIZACION ↔ USUARIO (1:N) -----
            builder.Entity<Cotizacion>()
                .HasOne(c => c.Usuario)
                .WithMany() // si después quieres que el usuario tenga ICollection<Cotizacion> se puede agregar
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
