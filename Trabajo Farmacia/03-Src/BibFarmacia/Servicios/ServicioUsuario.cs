using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BibFarmacia.Aspectos;
using BibFarmacia.Clases;
using BibFarmacia.Interfaces;

namespace BibFarmacia.Servicios
{
    public class ServicioUsuario
    {
        private List<Usuario> usuarios;

        private readonly ICargadorUsuarios
            cargador;

        public ServicioUsuario(
            ICargadorUsuarios cargador)
        {
            this.cargador = cargador;

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
            return cargador.Cargar(
                ruta,
                usuarios);
        }
    }
}
