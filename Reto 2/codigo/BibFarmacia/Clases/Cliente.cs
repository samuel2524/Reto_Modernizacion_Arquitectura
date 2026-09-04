using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibFarmacia.Clases
{
    public class Cliente : Persona
    {
        public int Puntos { get; set; }
        public Convenio? Convenio { get; }

        public Cliente(string nombre, string cedula,
            string telefono, string correo)
            : base(nombre, cedula, telefono, correo)
        {
            Puntos = 0;
            Convenio = null;
        }

        public Cliente(string nombre, string cedula,
            string telefono, string correo,
            Convenio convenio)
            : this(nombre, cedula, telefono, correo)
        {
            Convenio = convenio;
        }

        public void AcumularPuntos(int puntos)
        {
            Puntos += puntos;
        }
    }
}
