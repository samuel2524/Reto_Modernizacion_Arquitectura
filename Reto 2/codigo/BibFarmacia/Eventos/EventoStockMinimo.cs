using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BibFarmacia.Clases;
namespace BibFarmacia.Eventos
{
    public class EventoStockMinimo
    {
        public delegate void DelegadoStock(
            string mensaje);

        public event DelegadoStock? StockMinimo;

        public void Disparar(
            Producto producto)
        {
            StockMinimo?.Invoke(
                $"ALERTA: stock mínimo de {producto.Nombre}");
        }
    }
}