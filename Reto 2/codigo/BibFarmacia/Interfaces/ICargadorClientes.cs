using BibFarmacia.Clases;

namespace BibFarmacia.Interfaces
{
    public interface ICargadorClientes
    {
        string Cargar(
            string ruta,
            ICollection<Cliente> destino);
    }
}
