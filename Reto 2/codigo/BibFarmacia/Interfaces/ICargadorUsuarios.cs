using BibFarmacia.Clases;

namespace BibFarmacia.Interfaces
{
    public interface ICargadorUsuarios
    {
        string Cargar(
            string ruta,
            ICollection<Usuario> destino);
    }
}
