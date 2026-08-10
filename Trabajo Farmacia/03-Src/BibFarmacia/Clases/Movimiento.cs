using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibFarmacia.Clases
{
    public class Movimiento
    {
        public DateTime Fecha { get; set; }
        public int Cantidad { get; set; }
        public string Tipo { get; set; }
        public Producto Producto { get; set; }

        public Movimiento(DateTime fecha,
            int cantidad,
            string tipo,
            Producto producto)
        {
            Fecha = fecha;
            Cantidad = cantidad;
            Tipo = tipo;
            Producto = producto;
        }
    }
}