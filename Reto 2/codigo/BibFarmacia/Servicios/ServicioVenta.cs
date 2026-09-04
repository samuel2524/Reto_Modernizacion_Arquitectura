using BibFarmacia.Clases;

namespace BibFarmacia.Servicios
{
    public class ServicioVenta
    {
        private readonly ServicioMovimiento
            servicioMovimiento;

        public ServicioVenta(
            ServicioMovimiento servicioMovimiento)
        {
            this.servicioMovimiento =
                servicioMovimiento;
        }

        public string Vender(
            Producto producto,
            int cantidad)
        {
            producto.Stock -= cantidad;

            Movimiento venta =
                new Movimiento(
                    DateTime.Now,
                    cantidad,
                    "Venta",
                    producto);

            servicioMovimiento
                .RegistrarMovimiento(
                    venta);

            return "Venta registrada";
        }
    }
}
