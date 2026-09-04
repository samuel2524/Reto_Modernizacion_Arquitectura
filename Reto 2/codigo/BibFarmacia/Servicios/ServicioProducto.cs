using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BibFarmacia.Clases;
using BibFarmacia.Interfaces;

namespace BibFarmacia.Servicios
{
    public class ServicioProducto
    {
        private readonly List<Producto> productos;

        private readonly ICargadorProductos
            cargador;

        public ServicioProducto(
            ICargadorProductos cargador)
        {
            this.cargador = cargador;

            productos = new List<Producto>();
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

        public string CargarDesdeArchivo(
            string ruta)
        {
            return cargador.Cargar(
                ruta,
                productos);
        }
    }
}
