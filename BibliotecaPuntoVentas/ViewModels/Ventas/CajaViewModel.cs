namespace BibliotecaPuntoVentas.ViewModels.Ventas
{
    public class CajaViewModel
    {
        public Guid Id { get; set; }

        public string Folio { get; set; } = null!;

        public string UsuarioApertura { get; set; } = null!;

        public decimal MontoInicial { get; set; }

        public decimal TotalVentas { get; set; }

        public decimal TotalEfectivo { get; set; }

        public decimal TotalTarjeta { get; set; }

        public decimal TotalTransferencia { get; set; }

        public decimal EfectivoEsperado { get; set; }

        public DateTime FechaApertura { get; set; }

        public DateTime? FechaCierre { get; set; }

        public bool Abierta { get; set; }

        public int CantidadVentas { get; set; }
    }
}
