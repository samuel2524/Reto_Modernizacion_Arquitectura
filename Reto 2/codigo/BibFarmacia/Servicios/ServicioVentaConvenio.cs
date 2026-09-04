using BibFarmacia.Clases;
using BibFarmacia.Enum;
using BibFarmacia.Interfaces;

namespace BibFarmacia.Servicios
{
    public class ServicioVentaConvenio
    {
        private readonly IDictionary<
            TipoBeneficioConvenio,
            IPoliticaConvenio> politicas;

        private readonly ServicioMovimiento
            servicioMovimiento;

        public ServicioVentaConvenio(
            IDictionary<TipoBeneficioConvenio,
                IPoliticaConvenio> politicas,
            ServicioMovimiento servicioMovimiento)
        {
            this.politicas = politicas;
            this.servicioMovimiento =
                servicioMovimiento;
        }

        public ResultadoConvenio VenderConConvenio(
            Cliente cliente,
            Producto producto,
            int cantidad)
        {
            if (cantidad <= 0)
            {
                return new ResultadoConvenio(
                    false, 0m, 0m, 0m,
                    "Cantidad inválida");
            }

            Convenio? convenio = cliente.Convenio;

            if (convenio == null)
            {
                return new ResultadoConvenio(
                    false, 0m, 0m, 0m,
                    "Cliente sin convenio");
            }

            if (!politicas.TryGetValue(
                convenio.TipoBeneficio,
                out IPoliticaConvenio? politica))
            {
                return new ResultadoConvenio(
                    false, 0m, 0m,
                    convenio.CupoDisponible,
                    "Estrategia de convenio no registrada");
            }

            if (producto.Stock < cantidad)
            {
                return new ResultadoConvenio(
                    false, 0m, 0m,
                    convenio.CupoDisponible,
                    "Stock insuficiente");
            }

            decimal subtotal =
                producto.Precio * cantidad;

            SolicitudConvenio solicitud =
                new SolicitudConvenio(
                    subtotal,
                    convenio.Porcentaje,
                    convenio.CupoDisponible);

            ResultadoConvenio resultado =
                politica.Evaluar(solicitud);

            if (!resultado.Aprobado)
            {
                return resultado;
            }

            producto.Stock -= cantidad;
            convenio.CupoDisponible =
                resultado.CupoRestante;

            Movimiento venta =
                new Movimiento(
                    DateTime.Now,
                    cantidad,
                    "Venta",
                    producto);

            servicioMovimiento
                .RegistrarMovimiento(venta);

            return resultado;
        }
    }
}
