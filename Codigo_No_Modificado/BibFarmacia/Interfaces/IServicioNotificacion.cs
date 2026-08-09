using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibFarmacia.Interfaces
{
    public interface IServicioNotificacion
    {
        void EnviarNotificacion(string mensaje);
    }
}
