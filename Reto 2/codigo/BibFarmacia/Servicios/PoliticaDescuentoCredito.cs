using BibFarmacia.Clases;
using BibFarmacia.Interfaces;

namespace BibFarmacia.Servicios
{
    public class PoliticaDescuentoCredito :
        IPoliticaConvenio
    {
        public ResultadoConvenio Evaluar(
            SolicitudConvenio solicitud)
        {
            decimal descuento =
                solicitud.Subtotal *
                solicitud.Porcentaje / 100m;

            decimal total =
                solicitud.Subtotal - descuento;

            if (solicitud.CupoDisponible < total)
            {
                return new ResultadoConvenio(
                    false,
                    descuento,
                    total,
                    solicitud.CupoDisponible,
                    "Cupo insuficiente");
            }

            return new ResultadoConvenio(
                true,
                descuento,
                total,
                solicitud.CupoDisponible - total,
                null);
        }
    }
}
