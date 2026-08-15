using BibliotecaPuntoVentas.Helpers;
using BibliotecaPuntoVentas.Models;
using BibliotecaPuntoVentas.Models.Negocio;
using BibliotecaPuntoVentas.Models.Seguridad;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaPuntoVentas.Data
{
    public class DbInitializer
    {
        public static async Task InicializarAsync(
           IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

            await context.Database.MigrateAsync();

            await CrearUsuarioSistemaAsync(context);
            await CrearMetodosPagoAsync(context);
            await CrearCategoriasAsync(context);
        }

        private static async Task CrearUsuarioSistemaAsync(
            ApplicationDbContext context)
        {
            var existeUsuario = await context.Users
                .AnyAsync(u =>
                    u.Id == SistemaConstantes.UsuarioSistemaId);

            if (existeUsuario)
            {
                return;
            }

            var usuarioSistema = new ApplicationUser
            {
                Id = SistemaConstantes.UsuarioSistemaId,

                UserName = "sistema@novapos.local",
                NormalizedUserName = "SISTEMA@NOVAPOS.LOCAL",

                Email = "sistema@novapos.local",
                NormalizedEmail = "SISTEMA@NOVAPOS.LOCAL",

                EmailConfirmed = true,

                Nombre = "Usuario",
                ApellidoPaterno = "Sistema",

                Estatus = true,
                AltaSistema = DateTime.UtcNow,

                SecurityStamp = Guid.NewGuid().ToString()
            };

            await context.Users.AddAsync(usuarioSistema);
            await context.SaveChangesAsync();
        }

        private static async Task CrearMetodosPagoAsync(
            ApplicationDbContext context)
        {
            if (await context.MetodosPago.AnyAsync())
            {
                return;
            }

            var metodosPago = new List<MetodoPago>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Nombre = "Efectivo",
                    Descripcion = "Pago realizado en efectivo.",
                    Estatus = true,
                    AltaSistema = DateTime.UtcNow
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Nombre = "Tarjeta",
                    Descripcion = "Pago realizado con tarjeta.",
                    Estatus = true,
                    AltaSistema = DateTime.UtcNow
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Nombre = "Transferencia",
                    Descripcion = "Pago realizado por transferencia.",
                    Estatus = true,
                    AltaSistema = DateTime.UtcNow
                }
            };

            await context.MetodosPago.AddRangeAsync(metodosPago);
            await context.SaveChangesAsync();
        }

        private static async Task CrearCategoriasAsync(
            ApplicationDbContext context)
        {
            if (await context.CategoriasProducto.AnyAsync())
            {
                return;
            }

            var categorias = new List<CategoriaProducto>
            {
                new()
                {
                    Id = new Guid("A5D9874E-EC1A-4049-847B-143E0E8D5DB8"),
                    Nombre = "Prendas",
                    Descripcion = "Categoría para todo el inventario textil de vestir. Sirve para agrupar los reportes de ventas globales del departamento de ropa y aislar estos artículos de otras unidades de negocio. ",
                    Estatus = true,
                    AltaSistema = DateTime.Now
                },
                new()
                {
                    Id = new Guid("438C71F8-9C7B-4447-A98B-8333550B2243"),
                    Nombre = "Camisas",
                    Descripcion = "Comprende exclusivamente artículos superiores con cuello estructurado y botonadura completa (manga corta o larga).",
                    Estatus = true,
                    AltaSistema = DateTime.Now
                },
            };

            await context.CategoriasProducto
                .AddRangeAsync(categorias);

            await context.SaveChangesAsync();
        }
    }
}
