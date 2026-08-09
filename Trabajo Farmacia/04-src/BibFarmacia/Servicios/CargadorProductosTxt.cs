using BibFarmacia.Clases;
using BibFarmacia.Interfaces;

namespace BibFarmacia.Servicios
{
    public class CargadorProductosTxt :
        ICargadorProductos
    {
        private readonly ISelectorCreadorProducto
            selector;

        public CargadorProductosTxt(
            ISelectorCreadorProducto selector)
        {
            this.selector = selector;
        }

        public string Cargar(
            string ruta,
            ICollection<Producto> destino)
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

                    DatosProducto datosProducto =
                        new DatosProducto(
                            datos[0],
                            datos[1],
                            decimal.Parse(datos[2]),
                            int.Parse(datos[3]),
                            int.Parse(datos[4]),
                            DateTime.Parse(datos[5]),
                            datos[6..]);

                    ICreadorProducto creador =
                        selector.Seleccionar(
                            datosProducto.Tipo);

                    Producto producto =
                        creador.Crear(
                            datosProducto);

                    destino.Add(producto);
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
