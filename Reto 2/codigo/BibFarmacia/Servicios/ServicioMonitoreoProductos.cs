using BibFarmacia.Clases;
using BibFarmacia.Eventos;

namespace BibFarmacia.Servicios
{
    public class ServicioMonitoreoProductos
    {
        public EventoStockMinimo EventoStock;
        public EventoVencimiento EventoVencimiento;

        public ServicioMonitoreoProductos()
        {
            EventoStock =
                new EventoStockMinimo();

            EventoVencimiento =
                new EventoVencimiento();
        }

        public void VerificarStock(
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

        public void VerificarVencimiento(
            IEnumerable<Producto> productos)
        {
            foreach (var producto in productos)
            {
                int dias =
                    (producto.FechaVencimiento -
                    DateTime.Now).Days;

                if (dias <= 30)
                {
                    EventoVencimiento
                        .Disparar(producto);
                }
            }
        }
    }
}
