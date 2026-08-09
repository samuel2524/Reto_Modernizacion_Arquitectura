using BibFarmacia.Clases;

namespace BibFarmacia.Servicios
{
    public class ServicioVenta
    {
        private readonly ServicioProducto
            servicioProducto;

        private readonly ServicioMovimiento
            servicioMovimiento;

        public ServicioVenta(
            ServicioProducto servicioProducto,
            ServicioMovimiento servicioMovimiento)
        {
            this.servicioProducto =
                servicioProducto;

            this.servicioMovimiento =
                servicioMovimiento;
        }

        public Producto? BuscarProducto(
            string nombre)
        {
            return servicioProducto
                .ObtenerProductos()
                .FirstOrDefault(p =>
                    p.Nombre.ToLower()
                    .Contains(
                        nombre.ToLower()));
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
