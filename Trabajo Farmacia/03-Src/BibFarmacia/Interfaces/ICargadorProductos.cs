using BibFarmacia.Clases;

namespace BibFarmacia.Interfaces
{
    public interface ICargadorProductos
    {
        string Cargar(
            string ruta,
            ICollection<Producto> destino);
    }
}
