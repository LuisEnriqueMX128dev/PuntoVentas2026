using BibliotecaPuntoVentas.Data;
using BibliotecaPuntoVentas.Models;
using BibliotecaPuntoVentas.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<INovaPosService, NovaPosService>();

// El JavaScript del punto de venta envía el token antifalsificación en este encabezado.
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "RequestVerificationToken";
});

var app = builder.Build();

//para guardar las fotos del producto
var rutaFotosProductos =
    builder.Configuration["Archivos:RutaFotosProductos"]
    ?? throw new InvalidOperationException(
        "No se configuró Archivos:RutaFotosProductos.");
Directory.CreateDirectory(rutaFotosProductos);


//para agregar datos de pruebas al iniciar el programa
await DbInitializer.InicializarAsync(app.Services);
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

//igual funcion para guardar fotos
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        rutaFotosProductos),

    RequestPath = "/FotoProductos"
});

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Ventas}/{action=Index}/{id?}");

app.Run();
