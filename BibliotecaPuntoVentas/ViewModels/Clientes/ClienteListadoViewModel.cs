namespace BibliotecaPuntoVentas.ViewModels.Clientes
{
    public class ClienteListadoViewModel
    {
        public Guid Id { get; set; }

        public string NombreCompleto { get; set; } = null!;

        public string? NumeroTelefonico { get; set; }

        public string? CorreoElectronico { get; set; }

        public string? Direccion { get; set; }

        public int CantidadCompras { get; set; }

        public decimal TotalComprado { get; set; }

        public DateTime? FechaUltimaCompra { get; set; }

        public bool Estatus { get; set; }

        public string TipoCliente =>
            CantidadCompras >= 10
                ? "Frecuente"
                : "Activo";
    }
}
