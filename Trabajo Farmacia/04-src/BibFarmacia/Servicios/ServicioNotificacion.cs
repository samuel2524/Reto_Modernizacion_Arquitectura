using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BibFarmacia.Interfaces;

namespace BibFarmacia.Servicios
{
    public class ServicioNotificacion : IServicioNotificacion
    {
        public void EnviarNotificacion(string mensaje)
        {
            Console.WriteLine($"[NOTIFICACION] {mensaje}");
        }
    }
}