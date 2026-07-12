using InventarioBI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InventarioBI.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>   
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Tus tablas
        public DbSet<Producto> Productos { get; set; } = null!;
        public DbSet<Categoria> Categorias { get; set; } = null!;
        public DbSet<Marca> Marcas { get; set; } = null!;
        public DbSet<MovimientoInventario> MovimientosInventario { get; set; } = null!;
        public DbSet<ConteoFisico> ConteosFisicos { get; set; } = null!;
        public DbSet<Tienda> Tiendas { get; set; } = null!;
        public DbSet<Auditoria> Auditorias { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);   

            // Configuración de precisión decimal
            modelBuilder.Entity<Producto>()
                .Property(p => p.PrecioCosto).HasPrecision(18, 3);
            modelBuilder.Entity<Producto>()
                .Property(p => p.PrecioVenta).HasPrecision(18, 3);

            modelBuilder.Entity<MovimientoInventario>()
                .Property(m => m.Cantidad).HasPrecision(18, 3);
            modelBuilder.Entity<MovimientoInventario>()
                .Property(m => m.StockAnterior).HasPrecision(18, 3);
            modelBuilder.Entity<MovimientoInventario>()
                .Property(m => m.StockNuevo).HasPrecision(18, 3);

            modelBuilder.Entity<ConteoFisico>()
                .Property(c => c.StockSistema).HasPrecision(18, 3);
            modelBuilder.Entity<ConteoFisico>()
                .Property(c => c.StockFisico).HasPrecision(18, 3);

            modelBuilder.Entity<Tienda>()
                .Property(t => t.AreaM2).HasPrecision(18, 2);
        }
    }
}