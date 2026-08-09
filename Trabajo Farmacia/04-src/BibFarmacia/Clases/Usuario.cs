using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BibFarmacia.Clases
{
    public class Usuario : Persona
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public Usuario(string nombre, string cedula,
            string telefono, string correo,
            string userName, string password)
            : base(nombre, cedula, telefono, correo)
        {
            UserName = userName;
            Password = password;
        }
    }
}
