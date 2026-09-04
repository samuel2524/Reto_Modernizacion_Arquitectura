using BibFarmacia.Clases;
using BibFarmacia.Interfaces;

namespace BibFarmacia.Servicios
{
    public class CargadorUsuariosTxt :
        ICargadorUsuarios
    {
        public string Cargar(
            string ruta,
            ICollection<Usuario> destino)
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

                    Usuario usuario =
                        new Usuario(
                            datos[0],
                            datos[1],
                            datos[2],
                            datos[3],
                            datos[4],
                            datos[5]);

                    destino.Add(usuario);
                }

                return "Usuarios cargados";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
