using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibFarmacia.Clases
{
    public class Laboratorio
    {
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }

        public Laboratorio(string nombre,
            string direccion,
            string telefono)
        {
            Nombre = nombre;
            Direccion = direccion;
            Telefono = telefono;
        }
    }
}
