using BibliotecaPuntoVentas.Models.Negocio;
using BibliotecaPuntoVentas.Models.Seguridad;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaPuntoVentas.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Negocio
        public DbSet<CategoriaProducto> CategoriasProducto { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetallesVenta { get; set; }
        public DbSet<MetodoPago> MetodosPago { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<MovimientoInventario> MovimientosInventario { get; set; }
        public DbSet<Caja> Cajas { get; set; }
        public DbSet<CorteCaja> CortesCaja { get; set; }

        // Configuración
        public DbSet<ConfiguracionNegocio> ConfiguracionesNegocio { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            /*
             * SEGURIDAD CON SUS ESQUEMAS
             */
            builder.Entity<ApplicationUser>()
                .ToTable("Usuarios", "Seguridad");

            builder.Entity<IdentityRole>()
                .ToTable("Roles", "Seguridad");

            builder.Entity<IdentityUserRole<string>>()
                .ToTable("UsuariosRoles", "Seguridad");

            builder.Entity<IdentityUserClaim<string>>()
                .ToTable("UsuariosClaims", "Seguridad");

            builder.Entity<IdentityUserLogin<string>>()
                .ToTable("UsuariosLogins", "Seguridad");

            builder.Entity<IdentityRoleClaim<string>>()
                .ToTable("RolesClaims", "Seguridad");

            builder.Entity<IdentityUserToken<string>>()
                .ToTable("UsuariosTokens", "Seguridad");


            /*
             * TABLAS DE NEGOCIO CON SUS ESQUEMAS
             */
            builder.Entity<CategoriaProducto>()
                .ToTable("CategoriasProducto", "Negocio");

            builder.Entity<Producto>()
                .ToTable("Productos", "Negocio");

            builder.Entity<Cliente>()
                .ToTable("Clientes", "Negocio");

            builder.Entity<Venta>()
                .ToTable("Ventas", "Negocio");

            builder.Entity<DetalleVenta>()
                .ToTable("DetallesVenta", "Negocio");

            builder.Entity<MetodoPago>()
                .ToTable("MetodosPago", "Negocio");

            builder.Entity<Pago>()
                .ToTable("Pagos", "Negocio");

            builder.Entity<MovimientoInventario>()
                .ToTable("MovimientosInventario", "Negocio");

            builder.Entity<Caja>()
                .ToTable("Cajas", "Negocio");

            builder.Entity<CorteCaja>()
                .ToTable("CortesCaja", "Negocio");


            /*
             * CONFIGURACIÓN CON SUS ESQUEMAS
             */
            builder.Entity<ConfiguracionNegocio>()
                .ToTable("ConfiguracionesNegocio", "Configuracion");


            /*
             * RELACIONES DE PRODUCTOS
             */

            builder.Entity<Producto>()
                .HasOne(p => p.CategoriaProducto)
                .WithMany(c => c.Productos)
                .HasForeignKey(p => p.CategoriaProductoId)
                .OnDelete(DeleteBehavior.Restrict);


            /*
             * RELACIONES DE VENTA
             */

            builder.Entity<Venta>()
                .HasOne(v => v.Cliente)
                .WithMany(c => c.Ventas)
                .HasForeignKey(v => v.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Venta>()
                .HasOne(v => v.Usuario)
                .WithMany(u => u.Ventas)
                .HasForeignKey(v => v.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Venta>()
                .HasOne(v => v.Caja)
                .WithMany(c => c.Ventas)
                .HasForeignKey(v => v.CajaId)
                .OnDelete(DeleteBehavior.Restrict);


            /*
             * RELACIONES DE DETALLE DE VENTA
             */

            builder.Entity<DetalleVenta>()
                .HasOne(d => d.Venta)
                .WithMany(v => v.DetallesVenta)
                .HasForeignKey(d => d.VentaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<DetalleVenta>()
                .HasOne(d => d.Producto)
                .WithMany(p => p.DetallesVenta)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);


            /*
             * RELACIONES DE PAGOS
             */

            builder.Entity<Pago>()
                .HasOne(p => p.Venta)
                .WithMany(v => v.Pagos)
                .HasForeignKey(p => p.VentaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Pago>()
                .HasOne(p => p.MetodoPago)
                .WithMany(m => m.Pagos)
                .HasForeignKey(p => p.MetodoPagoId)
                .OnDelete(DeleteBehavior.Restrict);


            /*
             * RELACIONES DE INVENTARIO
             */

            builder.Entity<MovimientoInventario>()
                .HasOne(m => m.Producto)
                .WithMany(p => p.MovimientosInventario)
                .HasForeignKey(m => m.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<MovimientoInventario>()
                .HasOne(m => m.Usuario)
                .WithMany(u => u.MovimientosInventario)
                .HasForeignKey(m => m.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);


            /*
             * RELACIONES DE CAJA
             */

            builder.Entity<Caja>()
                .HasOne(c => c.UsuarioApertura)
                .WithMany(u => u.CajasAperturadas)
                .HasForeignKey(c => c.UsuarioAperturaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CorteCaja>()
                .HasOne(c => c.Caja)
                .WithMany(c => c.CortesCaja)
                .HasForeignKey(c => c.CajaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CorteCaja>()
                .HasOne(c => c.Usuario)
                .WithMany(u => u.CortesCaja)
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);


            /*
             * CONFIGURACIÓN DE DECIMALES
             */

            builder.Entity<Producto>()
                .Property(p => p.PrecioCompra)
                .HasPrecision(18, 2);

            builder.Entity<Producto>()
                .Property(p => p.PrecioVenta)
                .HasPrecision(18, 2);

            builder.Entity<Venta>()
                .Property(v => v.Subtotal)
                .HasPrecision(18, 2);

            builder.Entity<Venta>()
                .Property(v => v.Descuento)
                .HasPrecision(18, 2);

            builder.Entity<Venta>()
                .Property(v => v.Impuesto)
                .HasPrecision(18, 2);

            builder.Entity<Venta>()
                .Property(v => v.Total)
                .HasPrecision(18, 2);

            builder.Entity<DetalleVenta>()
                .Property(d => d.PrecioUnitario)
                .HasPrecision(18, 2);

            builder.Entity<DetalleVenta>()
                .Property(d => d.Descuento)
                .HasPrecision(18, 2);

            builder.Entity<DetalleVenta>()
                .Property(d => d.Subtotal)
                .HasPrecision(18, 2);

            builder.Entity<Pago>()
                .Property(p => p.Monto)
                .HasPrecision(18, 2);

            builder.Entity<Pago>()
                .Property(p => p.MontoRecibido)
                .HasPrecision(18, 2);

            builder.Entity<Pago>()
                .Property(p => p.Cambio)
                .HasPrecision(18, 2);

            builder.Entity<ConfiguracionNegocio>()
                .Property(c => c.PorcentajeImpuesto)
                .HasPrecision(5, 2);


            /*
             * ÍNDICES ÚNICOS
             */

            builder.Entity<Producto>()
                .HasIndex(p => p.Codigo)
                .IsUnique();

            builder.Entity<Venta>()
                .HasIndex(v => v.Folio)
                .IsUnique();

            builder.Entity<Caja>()
                .HasIndex(c => c.Folio)
                .IsUnique();
        }
    }
}
