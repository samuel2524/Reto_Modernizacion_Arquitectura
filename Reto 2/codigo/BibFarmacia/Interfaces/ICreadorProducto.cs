using BibFarmacia.Clases;

namespace BibFarmacia.Interfaces
{
    public interface ICreadorProducto
    {
        Producto Crear(
            DatosProducto datos);
    }
}
