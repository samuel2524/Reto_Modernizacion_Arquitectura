using BibFarmacia.Clases;
using BibFarmacia.Interfaces;

namespace BibFarmacia.Servicios
{
    public class PoliticaSoloCredito :
        IPoliticaConvenio
    {
        public ResultadoConvenio Evaluar(
            SolicitudConvenio solicitud)
        {
            decimal total = solicitud.Subtotal;

            if (solicitud.CupoDisponible < total)
            {
                return new ResultadoConvenio(
                    false,
                    0m,
                    total,
                    solicitud.CupoDisponible,
                    "Cupo insuficiente");
            }

            return new ResultadoConvenio(
                true,
                0m,
                total,
                solicitud.CupoDisponible - total,
                null);
        }
    }
}
