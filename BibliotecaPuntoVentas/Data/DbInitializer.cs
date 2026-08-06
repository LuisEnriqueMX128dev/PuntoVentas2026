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
                    Id = Guid.NewGuid(),
                    Nombre = "Abarrotes",
                    Descripcion = "Productos de consumo general.",
                    Estatus = true,
                    AltaSistema = DateTime.UtcNow
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Nombre = "Bebidas",
                    Descripcion = "Bebidas y líquidos.",
                    Estatus = true,
                    AltaSistema = DateTime.UtcNow
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Nombre = "Snacks",
                    Descripcion = "Botanas, dulces y alimentos rápidos.",
                    Estatus = true,
                    AltaSistema = DateTime.UtcNow
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Nombre = "Lácteos",
                    Descripcion = "Leche, yogurt y productos derivados.",
                    Estatus = true,
                    AltaSistema = DateTime.UtcNow
                }
            };

            await context.CategoriasProducto
                .AddRangeAsync(categorias);

            await context.SaveChangesAsync();
        }
    }
}
