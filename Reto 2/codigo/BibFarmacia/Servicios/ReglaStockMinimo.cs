using BibFarmacia.Clases;
using BibFarmacia.Eventos;
using BibFarmacia.Interfaces;

namespace BibFarmacia.Servicios
{
    public class ReglaStockMinimo :
        IReglaAlerta
    {
        public EventoStockMinimo EventoStock;

        public ReglaStockMinimo()
        {
            EventoStock =
                new EventoStockMinimo();
        }

        public void Verificar(
            IEnumerable<Producto> productos)
        {
            foreach (var producto in productos)
            {
                if (producto.Stock <=
                    producto.StockMinimo)
                {
                    EventoStock.Disparar(producto);
                }
            }
        }
    }
}
