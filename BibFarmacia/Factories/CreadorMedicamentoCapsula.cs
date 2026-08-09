using BibFarmacia.Clases;
using BibFarmacia.Enum;
using BibFarmacia.Interfaces;

namespace BibFarmacia.Factories
{
    public class CreadorMedicamentoCapsula :
        ICreadorProducto
    {
        public Producto Crear(
            DatosProducto datos)
        {
            Laboratorio laboratorio =
                new Laboratorio(
                    datos.Extra[0],
                    "Medellin",
                    "4444444");

            return new MedicamentoCapsula(
                datos.Nombre,
                datos.Precio,
                datos.Stock,
                datos.StockMinimo,
                datos.FechaVencimiento,
                laboratorio,
                TipoRelleno.Gel);
        }
    }
}
