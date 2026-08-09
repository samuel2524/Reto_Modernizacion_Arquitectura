using BibFarmacia.Clases;
using BibFarmacia.Interfaces;

namespace BibFarmacia.Factories
{
    public class CreadorCosmetico :
        ICreadorProducto
    {
        public Producto Crear(
            DatosProducto datos)
        {
            return new Cosmetico(
                datos.Nombre,
                datos.Precio,
                datos.Stock,
                datos.StockMinimo,
                datos.FechaVencimiento,
                datos.Extra[0],
                datos.Extra[1]);
        }
    }
}
