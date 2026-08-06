namespace BibliotecaPuntoVentas.Models.Negocio
{
    public class Cliente
    {
        public Guid Id { get; set; }

        public string Nombre { get; set; } = null!;
        public string? ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }

        public string? NumeroTelefonico { get; set; }
        public string? CorreoElectronico { get; set; }
        public string? Direccion { get; set; }

        public bool Estatus { get; set; } = true;

        public DateTime AltaSistema { get; set; } = DateTime.Now;
        public DateTime? ModificacionSistema { get; set; }

        // Relaciones
        public ICollection<Venta>? Ventas { get; set; }
    }
}
