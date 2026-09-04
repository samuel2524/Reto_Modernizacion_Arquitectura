using BibFarmacia.Clases;
using BibFarmacia.Enum;
using BibFarmacia.Interfaces;

namespace BibFarmacia.Factories
{
    public class CreadorComestible :
        ICreadorProducto
    {
        public Producto Crear(
            DatosProducto datos)
        {
            CategoriaComestible categoria =
                System.Enum.Parse<
                    CategoriaComestible>(
                        datos.Extra[0]);

            return new Comestible(
                datos.Nombre,
                datos.Precio,
                datos.Stock,
                datos.StockMinimo,
                datos.FechaVencimiento,
                categoria);
        }
    }
}
