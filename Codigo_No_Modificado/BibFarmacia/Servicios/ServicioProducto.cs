using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BibFarmacia.Clases;
using BibFarmacia.Interfaces;
using BibFarmacia.Eventos;

namespace BibFarmacia.Servicios
{
    public class ServicioProducto
    {
        private readonly List<Producto> productos;

        public EventoStockMinimo EventoStock;
        public EventoVencimiento EventoVencimiento;

        public ServicioProducto()
        {
            productos = new List<Producto>();

            EventoStock = new EventoStockMinimo();
            EventoVencimiento = new EventoVencimiento();
        }

        public string AgregarProducto(
            Producto producto)
        {
            try
            {
                productos.Add(producto);

                return "Producto agregado";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public List<Producto> ObtenerProductos()
        {
            return productos;
        }

        public void VerificarStock()
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

        public void VerificarVencimiento()
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

        public string CargarDesdeArchivo(
            string ruta)
        {
            try
            {
                if (!File.Exists(ruta))
                {
                    return "Archivo no encontrado";
                }

                string[] lineas =
                    File.ReadAllLines(ruta);

                foreach (string linea in lineas)
                {
                    string[] datos =
                        linea.Split(';');

                    Laboratorio laboratorio =
                        new Laboratorio(
                            datos[5],
                            "Medellin",
                            "4444444");

                    MedicamentoCapsula medicamento =
                        new MedicamentoCapsula(
                            datos[0],
                            decimal.Parse(datos[1]),
                            int.Parse(datos[2]),
                            int.Parse(datos[3]),
                            DateTime.Parse(datos[4]),
                            laboratorio,
                            Enum.TipoRelleno.Gel);

                    productos.Add(medicamento);
                }

                return "Productos cargados";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}