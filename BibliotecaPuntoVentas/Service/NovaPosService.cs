using BibliotecaPuntoVentas.Helpers;
using BibliotecaPuntoVentas.Models;
using BibliotecaPuntoVentas.Models.Negocio;
using BibliotecaPuntoVentas.ViewModels.Clientes;
using BibliotecaPuntoVentas.ViewModels.Configuracion;
using BibliotecaPuntoVentas.ViewModels.Dashboard;
using BibliotecaPuntoVentas.ViewModels.Inventario;
using BibliotecaPuntoVentas.ViewModels.Productos;
using BibliotecaPuntoVentas.ViewModels.Reportes;
using BibliotecaPuntoVentas.ViewModels.Ventas;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace BibliotecaPuntoVentas.Service
{
    public class NovaPosService : INovaPosService
    {
        private readonly ApplicationDbContext _context;
        private readonly string _rutaFotosProductos;
        private readonly string _urlFotosProductos;

        private const long PesoMaximoFoto = 20 * 1024 * 1024;

        private static readonly HashSet<string>ExtensionesPermitidas =new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        };

        public NovaPosService(ApplicationDbContext context,IConfiguration configuration)
        {
            _context = context;

            _rutaFotosProductos =
                configuration["Archivos:RutaFotosProductos"]
                ?? throw new InvalidOperationException(
                    "No se configuró la ruta de fotografías.");

            _urlFotosProductos =
                configuration["Archivos:UrlFotosProductos"]
                ?? "/FotoProductos";

            _urlFotosProductos =
                _urlFotosProductos.TrimEnd('/');

            Directory.CreateDirectory(
                _rutaFotosProductos);
        }

        #region Dashboard

        public async Task<DashboardViewModel> ObtenerDashboardAsync(DateTime? fechaInicio = null,DateTime? fechaFin = null)
        {
            var hoy = DateTime.Today;

            var inicio = fechaInicio?.Date
                ?? hoy.AddDays(-6);

            var fin = fechaFin?.Date
                ?? hoy;

            if (inicio > fin)
            {
                throw new InvalidOperationException(
                    "La fecha inicial no puede ser mayor que la fecha final.");
            }

            /*
             * Sumamos un día y usamos <
             * para incluir completamente la fecha final.
             */
            var finExclusivo = fin.AddDays(1);

            var ventasRango = _context.Ventas
                .AsNoTracking()
                .Where(v =>
                    !v.Cancelada &&
                    v.FechaVenta >= inicio &&
                    v.FechaVenta < finExclusivo);

            var gananciaTotalFiltro =
            await _context.DetallesVenta
                .AsNoTracking()
                .Where(d =>
                    !d.Venta!.Cancelada &&
                    d.Venta.FechaVenta >= inicio &&
                    d.Venta.FechaVenta < finExclusivo)
                .SumAsync(d =>
                    (decimal?)(
                        d.Subtotal -
                        (d.Producto!.PrecioCompra * d.Cantidad)
                    ))
            ?? 0;

            var totalVendidoFiltro =
            await ventasRango
                .SumAsync(v => (decimal?)v.Total)
            ?? 0;

            var ventasHoy = await _context.Ventas
                .AsNoTracking()
                .Where(v =>
                    !v.Cancelada &&
                    v.FechaVenta >= hoy &&
                    v.FechaVenta < hoy.AddDays(1))
                .SumAsync(v => (decimal?)v.Total)
                ?? 0;

            var cantidadVentasHoy = await _context.Ventas
                .AsNoTracking()
                .CountAsync(v =>
                    !v.Cancelada &&
                    v.FechaVenta >= hoy &&
                    v.FechaVenta < hoy.AddDays(1));

            var ventasAyer = await _context.Ventas
                .AsNoTracking()
                .Where(v =>
                    !v.Cancelada &&
                    v.FechaVenta >= hoy.AddDays(-1) &&
                    v.FechaVenta < hoy)
                .SumAsync(v => (decimal?)v.Total)
                ?? 0;

            decimal porcentajeCambio = 0;

            if (ventasAyer > 0)
            {
                porcentajeCambio =
                    ((ventasHoy - ventasAyer) / ventasAyer) * 100;
            }
            else if (ventasHoy > 0)
            {
                porcentajeCambio = 100;
            }

            var ventasAgrupadas = await ventasRango
                .GroupBy(v => v.FechaVenta.Date)
                .Select(g => new
                {
                    Fecha = g.Key,
                    Total = g.Sum(v => v.Total),
                    CantidadVentas = g.Count()
                })
                .OrderBy(x => x.Fecha)
                .ToListAsync();

            /*
             * Creamos todos los días del rango.
             * Así también aparecen días sin ventas en la gráfica.
             */
            var ventasUltimosDias =
                Enumerable.Range(
                    0,
                    (fin - inicio).Days + 1)
                .Select(i =>
                {
                    var fecha = inicio.AddDays(i);

                    var venta =
                        ventasAgrupadas
                            .FirstOrDefault(v =>
                                v.Fecha == fecha);

                    return new DashboardVentaDiariaViewModel
                    {
                        Fecha = fecha,

                        Dia = fecha.ToString(
                            "dd/MM"),

                        Total =
                            venta?.Total ?? 0,

                        CantidadVentas =
                            venta?.CantidadVentas ?? 0
                    };
                })
                .ToList();

            var productosMasVendidos =
                await _context.DetallesVenta
                    .AsNoTracking()
                    .Where(d =>
                        !d.Venta!.Cancelada &&
                        d.Venta.FechaVenta >= inicio &&
                        d.Venta.FechaVenta < finExclusivo)
                    .GroupBy(d => new
                    {
                        d.ProductoId,
                        d.Producto!.Codigo,
                        d.Producto.Nombre,
                        Categoria =
                            d.Producto.CategoriaProducto!.Nombre
                    })
                    .Select(g =>
                        new DashboardProductoVendidoViewModel
                        {
                            ProductoId =
                                g.Key.ProductoId,

                            Codigo =
                                g.Key.Codigo,

                            Nombre =
                                g.Key.Nombre,

                            Categoria =
                                g.Key.Categoria,

                            CantidadVendida =
                                g.Sum(x => x.Cantidad),

                            TotalVendido =
                                g.Sum(x => x.Subtotal)
                        })
                    .OrderByDescending(x =>
                        x.CantidadVendida)
                    .Take(5)
                    .ToListAsync();

            var actividades =
                await _context.Ventas
                    .AsNoTracking()
                    .OrderByDescending(v =>
                        v.FechaVenta)
                    .Take(6)
                    .Select(v =>
                        new DashboardActividadViewModel
                        {
                            Titulo =
                                "Venta " + v.Folio,

                            Descripcion =
                                v.Cancelada
                                    ? "Venta cancelada"
                                    : "Venta completada por " +
                                      v.Total.ToString("C2"),

                            TipoActividad =
                                v.Cancelada
                                    ? "CANCELADA"
                                    : "VENTA",

                            Fecha =
                                v.FechaVenta,

                            Referencia =
                                v.Folio
                        })
                    .ToListAsync();

            return new DashboardViewModel
            {
                FechaInicio = inicio,
                FechaFin = fin,

                VentasHoy = ventasHoy,
                VentasAyer = ventasAyer,
                CantidadVentasHoy = cantidadVentasHoy,

                TotalProductos =
                    await _context.Productos
                        .CountAsync(p => p.Estatus),

                TotalClientes =
                    await _context.Clientes
                        .CountAsync(c => c.Estatus),

                ProductosStockBajo =
                    await _context.Productos
                        .CountAsync(p =>
                            p.Estatus &&
                            p.Existencia > 0 &&
                            p.Existencia <= p.StockMinimo),

                ProductosAgotados =
                    await _context.Productos
                        .CountAsync(p =>
                            p.Estatus &&
                            p.Existencia <= 0),

                PorcentajeCambioVentas =
                    porcentajeCambio,

                VentasUltimosDias =
                    ventasUltimosDias,

                ProductosMasVendidos =
                    productosMasVendidos,

                ActividadesRecientes =
                    actividades,
                TotalVendidoFiltro = totalVendidoFiltro,
                GananciaTotalFiltro = gananciaTotalFiltro
            };
        }

        #endregion

        #region Categorias

        public async Task<List<CategoriaProductoViewModel>> ObtenerCategoriasAsync()
        {
            return await _context.CategoriasProducto.AsNoTracking().OrderBy(c => c.Nombre).Select(c => new CategoriaProductoViewModel
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion,
                Estatus = c.Estatus,
                TotalProductos = _context.Productos.Count(p => p.CategoriaProductoId == c.Id)
            }).ToListAsync();
        }
        public async Task<CategoriaProductoFormularioViewModel?> ObtenerCategoriaPorIdAsync(Guid categoriaId)
        {
            return await _context.CategoriasProducto.AsNoTracking().Where(c => c.Id == categoriaId).Select(c => new CategoriaProductoFormularioViewModel
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion,
                Estatus = c.Estatus
            }).FirstOrDefaultAsync();
        }

        public async Task<Guid> CrearCategoriaAsync(CategoriaProductoFormularioViewModel model)
        {
            var nombre = model.Nombre.Trim();

            var existe = await _context.CategoriasProducto.AnyAsync(c => c.Nombre.ToUpper() == nombre.ToUpper());

            if (existe)
            {
                throw new InvalidOperationException("Ya existe una categoría con ese nombre.");
            }

            var categoria = new CategoriaProducto
            {
                Id = Guid.NewGuid(),
                Nombre = nombre,
                Descripcion = model.Descripcion?.Trim(),
                Estatus = model.Estatus,
                AltaSistema = DateTime.Now
            };

            await _context.CategoriasProducto.AddAsync(categoria);
            await _context.SaveChangesAsync();

            return categoria.Id;
        }

        public async Task<bool> EditarCategoriaAsync(CategoriaProductoFormularioViewModel model)
        {
            if (!model.Id.HasValue)
            {
                return false;
            }

            var categoria = await _context.CategoriasProducto.FirstOrDefaultAsync(c => c.Id == model.Id.Value);

            if (categoria is null)
            {
                return false;
            }

            var nombre = model.Nombre.Trim();

            var existe = await _context.CategoriasProducto.AnyAsync(c => c.Id != model.Id.Value && c.Nombre.ToUpper() == nombre.ToUpper());

            if (existe)
            {
                throw new InvalidOperationException("Ya existe otra categoría con ese nombre.");
            }

            categoria.Nombre = nombre;
            categoria.Descripcion = model.Descripcion?.Trim();
            categoria.Estatus = model.Estatus;
            categoria.ModificacionSistema = DateTime.Now;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CambiarEstatusCategoriaAsync(Guid categoriaId)
        {
            var categoria = await _context.CategoriasProducto.FirstOrDefaultAsync(c => c.Id == categoriaId);

            if (categoria is null)
            {
                return false;
            }

            categoria.Estatus = !categoria.Estatus;
            categoria.ModificacionSistema = DateTime.Now;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> EliminarCategoriaAsync(Guid categoriaId)
        {
            var categoria = await _context.CategoriasProducto.FirstOrDefaultAsync(c => c.Id == categoriaId);

            if (categoria is null)
            {
                return false;
            }

            var tieneProductos = await _context.Productos.AnyAsync(p => p.CategoriaProductoId == categoriaId);

            if (tieneProductos)
            {
                throw new InvalidOperationException("No puedes eliminar esta categoría porque tiene productos asociados.");
            }

            _context.CategoriasProducto.Remove(categoria);
            await _context.SaveChangesAsync();

            return true;
        }

        #endregion

        #region Productos

        public async Task<List<ProductoListadoViewModel>>
            ObtenerProductosAsync(
                string? busqueda = null,
                Guid? categoriaId = null,
                bool? estatus = null)
        {
            var consulta = _context.Productos
                .AsNoTracking()
                .Include(p => p.CategoriaProducto)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                var texto = busqueda.Trim();

                consulta = consulta.Where(p =>
                    p.Nombre.Contains(texto) ||
                    p.Codigo.Contains(texto));
            }

            if (categoriaId.HasValue)
            {
                consulta = consulta.Where(p =>
                    p.CategoriaProductoId ==
                    categoriaId.Value);
            }

            if (estatus.HasValue)
            {
                consulta = consulta.Where(p =>
                    p.Estatus == estatus.Value);
            }

            return await consulta
                .OrderBy(p => p.Nombre)
                .Select(p => new ProductoListadoViewModel
                {
                    Id = p.Id,
                    Codigo = p.Codigo,
                    Nombre = p.Nombre,

                    Categoria =
                        p.CategoriaProducto != null
                            ? p.CategoriaProducto.Nombre
                            : "Sin categoría",

                    PrecioCompra = p.PrecioCompra,
                    PrecioVenta = p.PrecioVenta,
                    Existencia = p.Existencia,
                    StockMinimo = p.StockMinimo,
                    UrlImagen = p.UrlImagen,
                    Estatus = p.Estatus
                })
                .ToListAsync();
        }

        public async Task<ProductoFormularioViewModel?>ObtenerProductoPorIdAsync(Guid productoId)
        {
            var producto = await _context.Productos
                .AsNoTracking()
                .Where(p => p.Id == productoId)
                .Select(p => new ProductoFormularioViewModel
                {
                    Id = p.Id,

                    CategoriaProductoId =
                        p.CategoriaProductoId,

                    Codigo = p.Codigo,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    PrecioCompra = p.PrecioCompra,
                    PrecioVenta = p.PrecioVenta,
                    Existencia = p.Existencia,
                    StockMinimo = p.StockMinimo,
                    UrlImagenActual = p.UrlImagen,
                    Estatus = p.Estatus
                })
                .FirstOrDefaultAsync();

            if (producto is null)
            {
                return null;
            }

            producto.Categorias =
                await ObtenerCategoriasSelectAsync(
                    producto.CategoriaProductoId);

            return producto;
        }

        public async Task<Guid> CrearProductoAsync(
            ProductoFormularioViewModel model,
            string? usuarioId = null)
        {
            usuarioId ??=
                SistemaConstantes.UsuarioSistemaId;

            var codigo =
                model.Codigo
                    .Trim()
                    .ToUpperInvariant();

            if (await ExisteCodigoProductoAsync(codigo))
            {
                throw new InvalidOperationException(
                    "Ya existe un producto con ese código.");
            }

            var categoriaExiste =
                await _context.CategoriasProducto
                    .AnyAsync(c =>
                        c.Id == model.CategoriaProductoId &&
                        c.Estatus);

            if (!categoriaExiste)
            {
                throw new InvalidOperationException(
                    "La categoría seleccionada no existe o está inactiva.");
            }

            string? nuevaUrlImagen = null;

            try
            {
                if (model.Foto is not null)
                {
                    nuevaUrlImagen =
                        await GuardarFotoProductoAsync(
                            model.Foto);
                }

                var producto = new Producto
                {
                    Id = Guid.NewGuid(),

                    CategoriaProductoId =
                        model.CategoriaProductoId,

                    Codigo = codigo,
                    Nombre = model.Nombre.Trim(),
                    Descripcion =
                        model.Descripcion?.Trim(),

                    PrecioCompra = model.PrecioCompra,
                    PrecioVenta = model.PrecioVenta,
                    Existencia = model.Existencia,
                    StockMinimo = model.StockMinimo,
                    UrlImagen = nuevaUrlImagen,
                    Estatus = model.Estatus,
                    AltaSistema = DateTime.Now
                };

                await _context.Productos.AddAsync(
                    producto);

                if (model.Existencia > 0)
                {
                    var movimientoInicial =
                        new MovimientoInventario
                        {
                            Id = Guid.NewGuid(),
                            ProductoId = producto.Id,
                            UsuarioId = usuarioId,
                            TipoMovimiento =
                                "ENTRADA_INICIAL",

                            Cantidad = model.Existencia,
                            ExistenciaAnterior = 0,
                            ExistenciaNueva =
                                model.Existencia,

                            Referencia =
                                "ALTA_PRODUCTO",

                            Observaciones =
                                "Existencia inicial registrada al crear el producto.",

                            FechaMovimiento =
                                DateTime.Now,

                            AltaSistema =
                                DateTime.Now
                        };

                    await _context
                        .MovimientosInventario
                        .AddAsync(movimientoInicial);
                }

                await _context.SaveChangesAsync();

                return producto.Id;
            }
            catch
            {
                if (!string.IsNullOrWhiteSpace(
                        nuevaUrlImagen))
                {
                    await EliminarFotoProductoAsync(
                        nuevaUrlImagen);
                }

                throw;
            }
        }

        public async Task<bool> EditarProductoAsync(ProductoFormularioViewModel model)
        {
            if (!model.Id.HasValue)
            {
                return false;
            }

            var producto =
                await _context.Productos
                    .FirstOrDefaultAsync(p =>
                        p.Id == model.Id.Value);

            if (producto is null)
            {
                return false;
            }

            var categoriaExiste =
                await _context.CategoriasProducto
                    .AnyAsync(c =>
                        c.Id == model.CategoriaProductoId &&
                        c.Estatus);

            if (!categoriaExiste)
            {
                throw new InvalidOperationException(
                    "La categoría seleccionada no existe o está inactiva.");
            }

            var codigo =
                model.Codigo
                    .Trim()
                    .ToUpperInvariant();

            if (await ExisteCodigoProductoAsync(
                    codigo,
                    producto.Id))
            {
                throw new InvalidOperationException(
                    "Ya existe otro producto con ese código.");
            }

            var urlImagenAnterior =
                producto.UrlImagen;

            string? nuevaUrlImagen = null;

            try
            {
                if (model.Foto is not null)
                {
                    nuevaUrlImagen =
                        await GuardarFotoProductoAsync(
                            model.Foto);

                    producto.UrlImagen =
                        nuevaUrlImagen;
                }
                else if (model.EliminarImagen)
                {
                    producto.UrlImagen = null;
                }

                producto.CategoriaProductoId =
                    model.CategoriaProductoId;

                producto.Codigo = codigo;
                producto.Nombre =
                    model.Nombre.Trim();

                producto.Descripcion =
                    model.Descripcion?.Trim();

                producto.PrecioCompra =
                    model.PrecioCompra;

                producto.PrecioVenta =
                    model.PrecioVenta;

                /*
                 * La existencia no se modifica aquí.
                 * Los cambios de stock se realizan desde
                 * el módulo de inventario para conservar
                 * el historial de movimientos.
                 */

                producto.StockMinimo =
                    model.StockMinimo;

                producto.Estatus =
                    model.Estatus;

                producto.ModificacionSistema =
                    DateTime.Now;

                await _context.SaveChangesAsync();

                var imagenFueReemplazada =
                    !string.IsNullOrWhiteSpace(
                        nuevaUrlImagen);

                if ((imagenFueReemplazada ||
                     model.EliminarImagen) &&
                    !string.IsNullOrWhiteSpace(
                        urlImagenAnterior))
                {
                    await EliminarFotoProductoAsync(
                        urlImagenAnterior);
                }

                return true;
            }
            catch
            {
                if (!string.IsNullOrWhiteSpace(
                        nuevaUrlImagen))
                {
                    await EliminarFotoProductoAsync(
                        nuevaUrlImagen);
                }

                throw;
            }
        }

        public async Task<bool> CambiarEstatusProductoAsync(
            Guid productoId)
        {
            var producto =
                await _context.Productos
                    .FirstOrDefaultAsync(p =>
                        p.Id == productoId);

            if (producto is null)
            {
                return false;
            }

            producto.Estatus =
                !producto.Estatus;

            producto.ModificacionSistema =
                DateTime.Now;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExisteCodigoProductoAsync(string codigo, Guid? productoId = null)
        {
            var codigoNormalizado = codigo.Trim().ToUpperInvariant();

            return await _context.Productos.AnyAsync(p =>
                p.Codigo == codigoNormalizado &&
                (!productoId.HasValue || p.Id != productoId.Value));
        }

        #endregion

        #region Inventario

        public async Task<InventarioIndexViewModel> ObtenerInventarioAsync(string? busqueda = null)
        {
            var consulta = _context.Productos
                .AsNoTracking()
                .Include(p => p.CategoriaProducto)
                .Where(p => p.Estatus)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                var texto = busqueda.Trim();
                consulta = consulta.Where(p =>
                    p.Nombre.Contains(texto) || p.Codigo.Contains(texto));
            }

            var productos = await consulta
                .OrderBy(p => p.Nombre)
                .Select(p => new InventarioProductoViewModel
                {
                    ProductoId = p.Id,
                    Codigo = p.Codigo,
                    Nombre = p.Nombre,
                    Categoria = p.CategoriaProducto != null
                        ? p.CategoriaProducto.Nombre
                        : "Sin categoría",
                    PrecioCompra = p.PrecioCompra,
                    PrecioVenta = p.PrecioVenta,
                    Existencia = p.Existencia,
                    StockMinimo = p.StockMinimo,
                    Estatus = p.Estatus
                })
                .ToListAsync();

            var todosLosProductos = await _context.Productos
                .AsNoTracking()
                .Where(p => p.Estatus)
                .ToListAsync();

            return new InventarioIndexViewModel
            {
                Busqueda = busqueda,
                ValorTotalInventario = todosLosProductos.Sum(p => p.PrecioCompra * p.Existencia),
                TotalUnidades = todosLosProductos.Sum(p => p.Existencia),
                TotalProductos = todosLosProductos.Count,
                ProductosStockBajo = todosLosProductos.Count(p =>
                    p.Existencia > 0 && p.Existencia <= p.StockMinimo),
                ProductosAgotados = todosLosProductos.Count(p => p.Existencia <= 0),
                Productos = productos,
                UltimosMovimientos = await ObtenerMovimientosInventarioAsync()
            };
        }

        public async Task<List<MovimientoInventarioViewModel>> ObtenerMovimientosInventarioAsync(
            Guid? productoId = null)
        {
            var consulta = _context.MovimientosInventario
                .AsNoTracking()
                .Include(m => m.Producto)
                .Include(m => m.Usuario)
                .AsQueryable();

            if (productoId.HasValue)
            {
                consulta = consulta.Where(m => m.ProductoId == productoId.Value);
            }

            return await consulta
                .OrderByDescending(m => m.FechaMovimiento)
                .Take(30)
                .Select(m => new MovimientoInventarioViewModel
                {
                    Id = m.Id,
                    ProductoId = m.ProductoId,
                    CodigoProducto = m.Producto != null ? m.Producto.Codigo : string.Empty,
                    NombreProducto = m.Producto != null ? m.Producto.Nombre : "Producto eliminado",
                    TipoMovimiento = m.TipoMovimiento,
                    Cantidad = m.Cantidad,
                    ExistenciaAnterior = m.ExistenciaAnterior,
                    ExistenciaNueva = m.ExistenciaNueva,
                    Referencia = m.Referencia,
                    Observaciones = m.Observaciones,
                    NombreUsuario = m.Usuario != null
                        ? m.Usuario.Nombre + " " + m.Usuario.ApellidoPaterno
                        : "Sistema",
                    FechaMovimiento = m.FechaMovimiento
                })
                .ToListAsync();
        }

        public async Task<bool> RegistrarEntradaInventarioAsync(
            MovimientoInventarioFormularioViewModel model,
            string? usuarioId = null)
        {
            usuarioId ??= SistemaConstantes.UsuarioSistemaId;

            await using var transaccion = await _context.Database.BeginTransactionAsync();

            try
            {
                var producto = await _context.Productos
                    .FirstOrDefaultAsync(p => p.Id == model.ProductoId && p.Estatus);

                if (producto is null)
                {
                    throw new InvalidOperationException("El producto no existe o está inactivo.");
                }

                var existenciaAnterior = producto.Existencia;
                producto.Existencia += model.Cantidad;
                producto.ModificacionSistema = DateTime.Now;

                await _context.MovimientosInventario.AddAsync(new MovimientoInventario
                {
                    Id = Guid.NewGuid(),
                    ProductoId = producto.Id,
                    UsuarioId = usuarioId,
                    TipoMovimiento = SistemaConstantes.MovimientoEntrada,
                    Cantidad = model.Cantidad,
                    ExistenciaAnterior = existenciaAnterior,
                    ExistenciaNueva = producto.Existencia,
                    Referencia = model.Referencia?.Trim(),
                    Observaciones = model.Observaciones?.Trim(),
                    FechaMovimiento = DateTime.Now,
                    AltaSistema = DateTime.Now
                });

                await _context.SaveChangesAsync();
                await transaccion.CommitAsync();
                return true;
            }
            catch
            {
                await transaccion.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> RegistrarAjusteInventarioAsync(
            MovimientoInventarioFormularioViewModel model,
            string? usuarioId = null)
        {
            usuarioId ??= SistemaConstantes.UsuarioSistemaId;

            if (model.TipoMovimiento != SistemaConstantes.MovimientoAjusteEntrada &&
                model.TipoMovimiento != SistemaConstantes.MovimientoAjusteSalida)
            {
                throw new InvalidOperationException("El tipo de ajuste no es válido.");
            }

            await using var transaccion = await _context.Database.BeginTransactionAsync();

            try
            {
                var producto = await _context.Productos
                    .FirstOrDefaultAsync(p => p.Id == model.ProductoId && p.Estatus);

                if (producto is null)
                {
                    throw new InvalidOperationException("El producto no existe o está inactivo.");
                }

                var existenciaAnterior = producto.Existencia;
                var esEntrada = model.TipoMovimiento == SistemaConstantes.MovimientoAjusteEntrada;
                var existenciaNueva = esEntrada
                    ? existenciaAnterior + model.Cantidad
                    : existenciaAnterior - model.Cantidad;

                if (existenciaNueva < 0)
                {
                    throw new InvalidOperationException("La salida supera la existencia disponible.");
                }

                producto.Existencia = existenciaNueva;
                producto.ModificacionSistema = DateTime.Now;

                await _context.MovimientosInventario.AddAsync(new MovimientoInventario
                {
                    Id = Guid.NewGuid(),
                    ProductoId = producto.Id,
                    UsuarioId = usuarioId,
                    TipoMovimiento = model.TipoMovimiento,
                    Cantidad = model.Cantidad,
                    ExistenciaAnterior = existenciaAnterior,
                    ExistenciaNueva = existenciaNueva,
                    Referencia = model.Referencia?.Trim(),
                    Observaciones = model.Observaciones?.Trim(),
                    FechaMovimiento = DateTime.Now,
                    AltaSistema = DateTime.Now
                });

                await _context.SaveChangesAsync();
                await transaccion.CommitAsync();
                return true;
            }
            catch
            {
                await transaccion.RollbackAsync();
                throw;
            }
        }

        #endregion

        #region Punto de venta y caja

        public async Task<PuntoVentaViewModel> ObtenerPuntoVentaAsync(
            string? busqueda = null,
            Guid? categoriaId = null)
        {
            var productosConsulta = _context.Productos
                .AsNoTracking()
                .Include(p => p.CategoriaProducto)
                .Where(p => p.Estatus)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                var texto = busqueda.Trim();
                productosConsulta = productosConsulta.Where(p =>
                    p.Nombre.Contains(texto) || p.Codigo.Contains(texto));
            }

            if (categoriaId.HasValue)
            {
                productosConsulta = productosConsulta.Where(p =>
                    p.CategoriaProductoId == categoriaId.Value);
            }

            var caja = await ObtenerCajaAbiertaAsync();
            var configuracion = await _context.ConfiguracionesNegocio
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Estatus);

            return new PuntoVentaViewModel
            {
                Busqueda = busqueda,
                CategoriaId = categoriaId,
                CajaId = caja?.Id,
                TieneCajaAbierta = caja is not null,
                CajaAbierta = caja,
                PorcentajeImpuesto = configuracion?.PorcentajeImpuesto ?? 11.5m,
                Productos = await productosConsulta
                    .OrderBy(p => p.Nombre)
                    .Select(p => new ProductoPuntoVentaViewModel
                    {
                        Id = p.Id,
                        CategoriaProductoId = p.CategoriaProductoId,
                        Codigo = p.Codigo,
                        Nombre = p.Nombre,
                        Categoria = p.CategoriaProducto != null
                            ? p.CategoriaProducto.Nombre
                            : "Sin categoría",
                        PrecioVenta = p.PrecioVenta,
                        Existencia = p.Existencia,
                        UrlImagen = p.UrlImagen
                    })
                    .ToListAsync(),
                Categorias = await _context.CategoriasProducto
                    .AsNoTracking()
                    .Where(c => c.Estatus)
                    .OrderBy(c => c.Nombre)
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Nombre,
                        Selected = categoriaId.HasValue && categoriaId.Value == c.Id
                    })
                    .ToListAsync(),
                Clientes = await _context.Clientes
                    .AsNoTracking()
                    .Where(c => c.Estatus)
                    .OrderBy(c => c.Nombre)
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = (c.Nombre + " " + c.ApellidoPaterno + " " + c.ApellidoMaterno).Trim()
                    })
                    .ToListAsync(),
                MetodosPago = await _context.MetodosPago
                    .AsNoTracking()
                    .Where(m => m.Estatus)
                    .OrderBy(m => m.Nombre)
                    .Select(m => new SelectListItem
                    {
                        Value = m.Id.ToString(),
                        Text = m.Nombre
                    })
                    .ToListAsync()
            };
        }

        public async Task<ProductoPuntoVentaViewModel?> ObtenerProductoPorCodigoAsync(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                return null;
            }

            var codigoNormalizado = codigo.Trim().ToUpperInvariant();

            return await _context.Productos
                .AsNoTracking()
                .Where(p => p.Estatus && p.Codigo == codigoNormalizado)
                .Select(p => new ProductoPuntoVentaViewModel
                {
                    Id = p.Id,
                    CategoriaProductoId = p.CategoriaProductoId,
                    Codigo = p.Codigo,
                    Nombre = p.Nombre,
                    Categoria = p.CategoriaProducto != null
                        ? p.CategoriaProducto.Nombre
                        : "Sin categoría",
                    PrecioVenta = p.PrecioVenta,
                    Existencia = p.Existencia,
                    UrlImagen = p.UrlImagen
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ResultadoVentaViewModel> RegistrarVentaAsync(RegistrarVentaViewModel model, string? usuarioId = null)
        {
            usuarioId ??= SistemaConstantes.UsuarioSistemaId;

            var resultado = new ResultadoVentaViewModel
            {
                Exitoso = false,
                Mensaje = "No fue posible registrar la venta."
            };

            if (model.Detalles.Count == 0)
            {
                resultado.Errores.Add("Debes agregar por lo menos un producto.");
                return resultado;
            }

            if (model.Pagos.Count == 0)
            {
                resultado.Errores.Add("Debes registrar al menos un pago.");
                return resultado;
            }

            await using var transaccion = await _context.Database.BeginTransactionAsync();

            try
            {
                var caja = await _context.Cajas.FirstOrDefaultAsync(c => c.Id == model.CajaId && c.Abierta && c.Estatus);

                if (caja is null)
                {
                    throw new InvalidOperationException("No existe una caja abierta para registrar la venta.");
                }

                var idsProductos = model.Detalles.Select(d => d.ProductoId).Distinct().ToList();

                var productos = await _context.Productos.Where(p => idsProductos.Contains(p.Id) && p.Estatus).ToListAsync();

                if (productos.Count != idsProductos.Count)
                {
                    throw new InvalidOperationException("Uno o más productos no existen o están inactivos.");
                }

                var configuracion = await _context.ConfiguracionesNegocio.AsNoTracking().FirstOrDefaultAsync(c => c.Estatus);

                var tasaImpuesto = configuracion?.PorcentajeImpuesto ?? 11.5m;
                var ahora = DateTime.Now;
                var detallesVenta = new List<DetalleVenta>();
                decimal importeLineas = 0m;

                foreach (var detalleModel in model.Detalles)
                {
                    var producto = productos.First(p => p.Id == detalleModel.ProductoId);

                    if (detalleModel.Cantidad <= 0)
                    {
                        throw new InvalidOperationException($"La cantidad de {producto.Nombre} no es válida.");
                    }

                    if (producto.Existencia < detalleModel.Cantidad)
                    {
                        throw new InvalidOperationException($"No hay existencia suficiente de {producto.Nombre}. Disponible: {producto.Existencia}.");
                    }

                    var precioUnitario = decimal.Round(producto.PrecioVenta, 2, MidpointRounding.AwayFromZero);
                    var importeBruto = decimal.Round(precioUnitario * detalleModel.Cantidad, 2, MidpointRounding.AwayFromZero);
                    var descuentoLinea = decimal.Round(Math.Clamp(detalleModel.Descuento, 0m, importeBruto), 2, MidpointRounding.AwayFromZero);
                    var importeLinea = decimal.Round(importeBruto - descuentoLinea, 2, MidpointRounding.AwayFromZero);

                    importeLineas += importeLinea;

                    detallesVenta.Add(new DetalleVenta
                    {
                        Id = Guid.NewGuid(),
                        ProductoId = producto.Id,
                        Cantidad = detalleModel.Cantidad,
                        PrecioUnitario = precioUnitario,
                        Descuento = descuentoLinea,
                        Subtotal = importeLinea,
                        AltaSistema = ahora
                    });
                }

                importeLineas = decimal.Round(importeLineas, 2, MidpointRounding.AwayFromZero);

                var descuentoGlobal = decimal.Round(Math.Clamp(model.Descuento, 0m, importeLineas), 2, MidpointRounding.AwayFromZero);
                var total = decimal.Round(importeLineas - descuentoGlobal, 2, MidpointRounding.AwayFromZero);
                var impuesto = tasaImpuesto > 0 ? decimal.Round(total - (total / (1m + (tasaImpuesto / 100m))), 2, MidpointRounding.AwayFromZero) : 0m;
                var subtotal = decimal.Round(total - impuesto, 2, MidpointRounding.AwayFromZero);

                var totalPagos = decimal.Round(model.Pagos.Sum(p => p.Monto), 2, MidpointRounding.AwayFromZero);

                if (totalPagos != total)
                {
                    throw new InvalidOperationException($"La suma de los pagos ({totalPagos:C2}) debe ser igual al total ({total:C2}).");
                }

                var idsMetodosPago = model.Pagos.Select(p => p.MetodoPagoId).Distinct().ToList();
                var metodosPago = await _context.MetodosPago.Where(m => idsMetodosPago.Contains(m.Id) && m.Estatus).ToDictionaryAsync(m => m.Id, m => m.Nombre);

                if (metodosPago.Count != idsMetodosPago.Count)
                {
                    throw new InvalidOperationException("Uno o más métodos de pago no son válidos.");
                }

                var ventaId = Guid.NewGuid();
                var folio = GenerarFolioVenta();

                var venta = new Venta
                {
                    Id = ventaId,
                    ClienteId = model.ClienteId,
                    CajaId = model.CajaId,
                    UsuarioId = usuarioId,
                    Folio = folio,
                    Subtotal = subtotal,
                    Descuento = decimal.Round(descuentoGlobal + detallesVenta.Sum(d => d.Descuento), 2, MidpointRounding.AwayFromZero),
                    Impuesto = impuesto,
                    Total = total,
                    Cancelada = false,
                    FechaVenta = ahora,
                    AltaSistema = ahora
                };

                foreach (var detalle in detallesVenta)
                {
                    detalle.VentaId = ventaId;
                }

                var pagos = new List<Pago>();
                decimal cambioTotal = 0m;
                decimal montoRecibidoTotal = 0m;

                foreach (var pagoModel in model.Pagos)
                {
                    var nombreMetodo = metodosPago[pagoModel.MetodoPagoId];
                    var esEfectivo = nombreMetodo.Equals("Efectivo", StringComparison.OrdinalIgnoreCase);
                    var montoPago = decimal.Round(pagoModel.Monto, 2, MidpointRounding.AwayFromZero);
                    var montoRecibido = decimal.Round(pagoModel.MontoRecibido > 0 ? pagoModel.MontoRecibido : montoPago, 2, MidpointRounding.AwayFromZero);

                    if (esEfectivo && montoRecibido < montoPago)
                    {
                        throw new InvalidOperationException($"El monto recibido ({montoRecibido:C2}) es menor al monto a cobrar ({montoPago:C2}).");
                    }

                    var cambio = esEfectivo ? decimal.Round(montoRecibido - montoPago, 2, MidpointRounding.AwayFromZero) : 0m;

                    pagos.Add(new Pago
                    {
                        Id = Guid.NewGuid(),
                        VentaId = ventaId,
                        MetodoPagoId = pagoModel.MetodoPagoId,
                        Monto = montoPago,
                        MontoRecibido = montoRecibido,
                        Cambio = cambio,
                        Referencia = pagoModel.Referencia?.Trim(),
                        Estatus = true,
                        FechaPago = ahora,
                        AltaSistema = ahora
                    });

                    cambioTotal += cambio;
                    montoRecibidoTotal += montoRecibido;
                }

                cambioTotal = decimal.Round(cambioTotal, 2, MidpointRounding.AwayFromZero);
                montoRecibidoTotal = decimal.Round(montoRecibidoTotal, 2, MidpointRounding.AwayFromZero);

                await _context.Ventas.AddAsync(venta);
                await _context.DetallesVenta.AddRangeAsync(detallesVenta);
                await _context.Pagos.AddRangeAsync(pagos);

                foreach (var detalleModel in model.Detalles)
                {
                    var producto = productos.First(p => p.Id == detalleModel.ProductoId);
                    var existenciaAnterior = producto.Existencia;

                    producto.Existencia -= detalleModel.Cantidad;
                    producto.ModificacionSistema = ahora;

                    await _context.MovimientosInventario.AddAsync(new MovimientoInventario
                    {
                        Id = Guid.NewGuid(),
                        ProductoId = producto.Id,
                        UsuarioId = usuarioId,
                        TipoMovimiento = SistemaConstantes.MovimientoSalidaVenta,
                        Cantidad = detalleModel.Cantidad,
                        ExistenciaAnterior = existenciaAnterior,
                        ExistenciaNueva = producto.Existencia,
                        Referencia = folio,
                        Observaciones = "Salida automática por venta.",
                        FechaMovimiento = ahora,
                        AltaSistema = ahora
                    });
                }

                await _context.SaveChangesAsync();
                await transaccion.CommitAsync();

                resultado.Exitoso = true;
                resultado.VentaId = ventaId;
                resultado.Folio = folio;
                resultado.Total = total;
                resultado.MontoRecibido = montoRecibidoTotal;
                resultado.Cambio = cambioTotal;
                resultado.Mensaje = "La venta se registró correctamente.";

                return resultado;
            }
            catch (Exception ex)
            {
                await transaccion.RollbackAsync();

                resultado.Errores.Add(ex.Message);

                return resultado;
            }
        }



        public async Task<CajaViewModel?> ObtenerCajaAbiertaAsync(string? usuarioId = null)
        {
            usuarioId ??= SistemaConstantes.UsuarioSistemaId;

            var caja = await _context.Cajas
                .AsNoTracking()
                .Include(c => c.UsuarioApertura)
                .Where(c => c.Abierta && c.Estatus && c.UsuarioAperturaId == usuarioId)
                .OrderByDescending(c => c.FechaApertura)
                .FirstOrDefaultAsync();

            if (caja is null)
            {
                return null;
            }

            var ventas = _context.Ventas
                .AsNoTracking()
                .Where(v => v.CajaId == caja.Id && !v.Cancelada);

            var pagos = _context.Pagos
                .AsNoTracking()
                .Where(p => p.Estatus && p.Venta!.CajaId == caja.Id && !p.Venta.Cancelada);

            var totalVentas = await ventas.SumAsync(v => (decimal?)v.Total) ?? 0m;
            var cantidadVentas = await ventas.CountAsync();

            var pagosAgrupados = await pagos
                .GroupBy(p => p.MetodoPago!.Nombre)
                .Select(g => new
                {
                    Metodo = g.Key,
                    Total = g.Sum(p => p.Monto)
                })
                .ToListAsync();

            decimal ObtenerTotal(string nombre) => pagosAgrupados
                .Where(p => p.Metodo.Equals(nombre, StringComparison.OrdinalIgnoreCase))
                .Sum(p => p.Total);

            var totalEfectivo = ObtenerTotal("Efectivo");

            return new CajaViewModel
            {
                Id = caja.Id,
                Folio = caja.Folio,
                UsuarioApertura = caja.UsuarioApertura != null
                    ? caja.UsuarioApertura.Nombre + " " + caja.UsuarioApertura.ApellidoPaterno
                    : "Sistema",
                MontoInicial = caja.MontoInicial,
                TotalVentas = totalVentas,
                TotalEfectivo = totalEfectivo,
                TotalTarjeta = ObtenerTotal("Tarjeta"),
                TotalTransferencia = ObtenerTotal("Transferencia"),
                EfectivoEsperado = caja.MontoInicial + totalEfectivo,
                FechaApertura = caja.FechaApertura,
                FechaCierre = caja.FechaCierre,
                Abierta = caja.Abierta,
                CantidadVentas = cantidadVentas
            };
        }

        public async Task<Guid> AbrirCajaAsync(
            AperturaCajaViewModel model,
            string? usuarioId = null)
        {
            usuarioId ??= SistemaConstantes.UsuarioSistemaId;

            var cajaExistente = await _context.Cajas.AnyAsync(c =>
                c.UsuarioAperturaId == usuarioId && c.Abierta && c.Estatus);

            if (cajaExistente)
            {
                throw new InvalidOperationException("Ya existe una caja abierta.");
            }

            var caja = new Caja
            {
                Id = Guid.NewGuid(),
                UsuarioAperturaId = usuarioId,
                Folio = "CAJ-" + DateTime.Now.ToString("yyyyMMdd-HHmmss"),
                MontoInicial = model.MontoInicial,
                FechaApertura = DateTime.Now,
                Abierta = true,
                Estatus = true,
                AltaSistema = DateTime.Now
            };

            await _context.Cajas.AddAsync(caja);
            await _context.SaveChangesAsync();
            return caja.Id;
        }

        public async Task<bool> CerrarCajaAsync(
            CierreCajaViewModel model,
            string? usuarioId = null)
        {
            usuarioId ??= SistemaConstantes.UsuarioSistemaId;

            var caja = await _context.Cajas
                .FirstOrDefaultAsync(c => c.Id == model.CajaId && c.Abierta && c.Estatus);

            if (caja is null)
            {
                return false;
            }

            var resumen = await ObtenerCajaAbiertaAsync(usuarioId);
            if (resumen is null)
            {
                return false;
            }

            await using var transaccion = await _context.Database.BeginTransactionAsync();

            try
            {
                await _context.CortesCaja.AddAsync(new CorteCaja
                {
                    Id = Guid.NewGuid(),
                    CajaId = caja.Id,
                    UsuarioId = usuarioId,
                    MontoInicial = resumen.MontoInicial,
                    TotalVentas = resumen.TotalVentas,
                    TotalEfectivo = resumen.TotalEfectivo,
                    TotalTarjeta = resumen.TotalTarjeta,
                    TotalTransferencia = resumen.TotalTransferencia,
                    EfectivoEsperado = resumen.EfectivoEsperado,
                    EfectivoContado = model.EfectivoContado,
                    Diferencia = model.EfectivoContado - resumen.EfectivoEsperado,
                    Observaciones = model.Observaciones?.Trim(),
                    FechaCorte = DateTime.Now,
                    AltaSistema = DateTime.Now
                });

                caja.Abierta = false;
                caja.FechaCierre = DateTime.Now;

                await _context.SaveChangesAsync();
                await transaccion.CommitAsync();
                return true;
            }
            catch
            {
                await transaccion.RollbackAsync();
                throw;
            }
        }

        #endregion

        #region Metodos aun no implementados

        public Task<List<ClienteListadoViewModel>> ObtenerClientesAsync(string? busqueda = null)
            => throw new NotImplementedException();

        public Task<ClienteFormularioViewModel?> ObtenerClientePorIdAsync(Guid clienteId)
            => throw new NotImplementedException();

        public Task<Guid> CrearClienteAsync(ClienteFormularioViewModel model)
            => throw new NotImplementedException();

        public Task<bool> EditarClienteAsync(ClienteFormularioViewModel model)
            => throw new NotImplementedException();

        public Task<bool> CambiarEstatusClienteAsync(Guid clienteId)
            => throw new NotImplementedException();

        public Task<List<VentaListadoViewModel>> ObtenerVentasAsync(
            DateTime? fechaInicio = null,
            DateTime? fechaFin = null)
            => throw new NotImplementedException();

        public Task<VentaDetalleViewModel?> ObtenerDetalleVentaAsync(Guid ventaId)
            => throw new NotImplementedException();

        public Task<bool> CancelarVentaAsync(
            Guid ventaId,
            string motivo,
            string? usuarioId = null)
            => throw new NotImplementedException();

        public Task<ReporteVentasViewModel> ObtenerReporteVentasAsync(
            DateTime fechaInicio,
            DateTime fechaFin)
            => throw new NotImplementedException();

        public Task<ReporteInventarioViewModel> ObtenerReporteInventarioAsync()
            => throw new NotImplementedException();

        public Task<ConfiguracionNegocioViewModel> ObtenerConfiguracionAsync()
            => throw new NotImplementedException();

        public Task<bool> GuardarConfiguracionAsync(ConfiguracionNegocioViewModel model)
            => throw new NotImplementedException();

        #endregion

        #region Auxiliares

        private async Task<List<SelectListItem>> ObtenerCategoriasSelectAsync(
            Guid? categoriaSeleccionada = null)
        {
            return await _context.CategoriasProducto
                .AsNoTracking()
                .Where(c => c.Estatus)
                .OrderBy(c => c.Nombre)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Nombre,
                    Selected = categoriaSeleccionada.HasValue &&
                               c.Id == categoriaSeleccionada.Value
                })
                .ToListAsync();
        }

        private static string GenerarFolioVenta()
        {
            var aleatorio = RandomNumberGenerator.GetInt32(1000, 9999);
            return $"NV-{DateTime.Now:yyyyMMddHHmmss}-{aleatorio}";
        }

        #endregion

        #region ElimarFotoProducto
        private async Task<string> GuardarFotoProductoAsync(IFormFile foto)
        {
            if (foto.Length <= 0)
            {
                throw new InvalidOperationException(
                    "El archivo de imagen está vacío.");
            }

            if (foto.Length > PesoMaximoFoto)
            {
                throw new InvalidOperationException(
                    "La fotografía no puede pesar más de 5 MB.");
            }

            var extension =
                Path.GetExtension(foto.FileName)
                    .ToLowerInvariant();

            if (!ExtensionesPermitidas.Contains(extension))
            {
                throw new InvalidOperationException(
                    "Solamente se permiten imágenes JPG, JPEG, PNG o WEBP.");
            }

            if (string.IsNullOrWhiteSpace(foto.ContentType) ||
                !foto.ContentType.StartsWith(
                    "image/",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "El archivo seleccionado no es una imagen válida.");
            }

            var nombreArchivo =
                $"{Guid.NewGuid():N}{extension}";

            var rutaCompleta =
                Path.Combine(
                    _rutaFotosProductos,
                    nombreArchivo);

            await using var stream = new FileStream(
                rutaCompleta,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None);

            await foto.CopyToAsync(stream);

            return $"{_urlFotosProductos}/{nombreArchivo}";
        }

        private Task EliminarFotoProductoAsync(string? urlImagen)
        {
            if (string.IsNullOrWhiteSpace(urlImagen))
            {
                return Task.CompletedTask;
            }

            try
            {
                var nombreArchivo =
                    Path.GetFileName(
                        urlImagen.Replace(
                            '/',
                            Path.DirectorySeparatorChar));

                if (string.IsNullOrWhiteSpace(nombreArchivo))
                {
                    return Task.CompletedTask;
                }

                var rutaCompleta =
                    Path.Combine(
                        _rutaFotosProductos,
                        nombreArchivo);

                if (File.Exists(rutaCompleta))
                {
                    File.Delete(rutaCompleta);
                }
            }
            catch
            {
                /*
                 * La información del producto ya puede estar
                 * guardada correctamente. Por eso un error al
                 * borrar una fotografía anterior no debe cancelar
                 * toda la operación.
                 */
            }

            return Task.CompletedTask;
        }
        #endregion

    }
}
