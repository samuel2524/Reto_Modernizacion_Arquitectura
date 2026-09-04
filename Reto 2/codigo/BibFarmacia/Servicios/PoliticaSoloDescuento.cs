using BibFarmacia.Clases;
using BibFarmacia.Interfaces;

namespace BibFarmacia.Servicios
{
    public class PoliticaSoloDescuento :
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

            return new ResultadoConvenio(
                true,
                descuento,
                total,
                solicitud.CupoDisponible,
                null);
        }
    }
}
