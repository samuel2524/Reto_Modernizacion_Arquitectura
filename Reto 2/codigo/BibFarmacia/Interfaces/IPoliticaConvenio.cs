using BibFarmacia.Clases;

namespace BibFarmacia.Interfaces
{
    public interface IPoliticaConvenio
    {
        ResultadoConvenio Evaluar(
            SolicitudConvenio solicitud);
    }
}
