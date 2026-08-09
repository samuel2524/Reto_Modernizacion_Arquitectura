using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BibFarmacia.Aspectos;
using BibFarmacia.Clases;

namespace BibFarmacia.Servicios
{
    public class ServicioUsuario
    {
        private List<Usuario> usuarios;

        public ServicioUsuario()
        {
            usuarios = new List<Usuario>();
        }

        public void AgregarUsuario(
            Usuario usuario)
        {
            usuarios.Add(usuario);
        }

        public bool Login(
            string user,
            string password)
        {
            return AspectoAutenticacion.Login(
                usuarios,
                user,
                password);
        }

        public string Cargar(
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

                    Usuario usuario =
                        new Usuario(
                            datos[0],
                            datos[1],
                            datos[2],
                            datos[3],
                            datos[4],
                            datos[5]);

                    usuarios.Add(usuario);
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