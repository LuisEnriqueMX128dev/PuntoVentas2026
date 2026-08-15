using BibliotecaPuntoVentas.Data;
using BibliotecaPuntoVentas.Models;
using BibliotecaPuntoVentas.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting.WindowsServices;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,

    // Cuando corre como servicio de Windows, evita que busque archivos desde C:\Windows\System32
    ContentRootPath = WindowsServiceHelpers.IsWindowsService()
        ? AppContext.BaseDirectory
        : Directory.GetCurrentDirectory()
});

// Nombre interno del servicio de Windows
builder.Host.UseWindowsService(options =>
{
    options.ServiceName = "TYLIAM_PRINT";
});

// Puerto local del sistema.
// El cliente abrirá: http://localhost:5050
builder.WebHost.UseUrls("http://localhost:5050");

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<INovaPosService, NovaPosService>();

// El JavaScript del punto de venta envía el token antifalsificación en este encabezado.
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "RequestVerificationToken";
});

var app = builder.Build();

// Carpeta externa para guardar fotos de productos.
// Ejemplo recomendado en appsettings.json:
// "RutaFotosProductos": "C:\\TYLIAM_PRINT\\FotoProductos"
var rutaFotosProductos =
    app.Configuration["Archivos:RutaFotosProductos"]
    ?? throw new InvalidOperationException(
        "No se configuró Archivos:RutaFotosProductos.");

var urlFotosProductos =
    app.Configuration["Archivos:UrlFotosProductos"]
    ?? "/FotoProductos";

// Crea la carpeta si no existe
Directory.CreateDirectory(rutaFotosProductos);

// Crea o actualiza la base de datos automáticamente.
// Esto ejecuta tus migraciones al iniciar.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    await context.Database.MigrateAsync();
}

// Ejecuta tus semillas / datos iniciales.
// Si tu DbInitializer ya hace MigrateAsync(), puedes quitar el bloque anterior.
await DbInitializer.InicializarAsync(app.Services);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

// Para servicio local en http://localhost:5050 normalmente NO uses HTTPS redirection.
// app.UseHttpsRedirection();

app.UseStaticFiles();

// Servir fotos externas.
// Si en BD guardas: /FotoProductos/archivo.png
// Esto buscará el archivo físico en: C:\TYLIAM_PRINT\FotoProductos\archivo.png
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(rutaFotosProductos),
    RequestPath = urlFotosProductos
});

app.UseRouting();

// Si tienes login con Identity o autenticación propia, activa esto:
// app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Ventas}/{action=Index}/{id?}");

app.Run();