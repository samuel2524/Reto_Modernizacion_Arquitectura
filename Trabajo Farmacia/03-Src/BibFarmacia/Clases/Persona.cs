using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibFarmacia.Clases
{
    public abstract class Persona
    {
        public string Nombre { get; set; }
        public string Cedula { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }

        protected Persona(string nombre, string cedula,
            string telefono, string correo)
        {
            Nombre = nombre;
            Cedula = cedula;
            Telefono = telefono;
            Correo = correo;
        }
    }
}