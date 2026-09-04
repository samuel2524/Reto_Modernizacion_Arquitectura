using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BibFarmacia.Clases
{
    public abstract class Producto
    {
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public int StockMinimo { get; set; }
        public DateTime FechaVencimiento { get; set; }

        protected Producto(string nombre,
            decimal precio,
            int stock,
            int stockMinimo,
            DateTime fechaVencimiento)
        {
            Nombre = nombre;
            Precio = precio;
            Stock = stock;
            StockMinimo = stockMinimo;
            FechaVencimiento = fechaVencimiento;
        }

        public virtual void MostrarInformacion()
        {
            Console.WriteLine($"Producto: {Nombre}");
            Console.WriteLine($"Precio: {Precio}");
            Console.WriteLine($"Stock: {Stock}");
        }
    }
}