namespace BibliotecaPuntoVentas.Models.Negocio
{
    public class Pago
    {
        public Guid Id { get; set; }

        public Guid VentaId { get; set; }
        public Guid MetodoPagoId { get; set; }

        public decimal Monto { get; set; }
        public decimal MontoRecibido { get; set; }
        public decimal Cambio { get; set; }

        public string? Referencia { get; set; }

        public bool Estatus { get; set; } = true;

        public DateTime FechaPago { get; set; } = DateTime.Now;
        public DateTime AltaSistema { get; set; } = DateTime.Now;

        // Relaciones
        public Venta? Venta { get; set; }
        public MetodoPago? MetodoPago { get; set; }
    }
}
