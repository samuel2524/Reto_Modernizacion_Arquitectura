using BibFarmacia.Clases;
using BibFarmacia.Eventos;

namespace BibFarmacia.Servicios
{
    public class ServicioFidelizacion
    {
        public EventoPuntos EventoPuntos;

        public ServicioFidelizacion()
        {
            EventoPuntos =
                new EventoPuntos();
        }

        public void AcumularPuntos(
            Cliente cliente,
            int puntos)
        {
            cliente.AcumularPuntos(
                puntos);

            EventoPuntos.Disparar(
                cliente.Nombre,
                puntos);
        }
    }
}
