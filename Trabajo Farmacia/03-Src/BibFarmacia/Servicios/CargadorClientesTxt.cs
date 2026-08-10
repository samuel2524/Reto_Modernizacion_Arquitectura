using BibFarmacia.Clases;
using BibFarmacia.Interfaces;

namespace BibFarmacia.Servicios
{
    public class CargadorClientesTxt :
        ICargadorClientes
    {
        public string Cargar(
            string ruta,
            ICollection<Cliente> destino)
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

                    Cliente cliente =
                        new Cliente(
                            datos[0],
                            datos[1],
                            datos[2],
                            datos[3]);

                    destino.Add(cliente);
                }

                return "Clientes cargados";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
