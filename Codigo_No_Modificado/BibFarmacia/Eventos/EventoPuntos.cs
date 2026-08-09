using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibFarmacia.Eventos
{
    public class EventoPuntos
    {
        public delegate void DelegadoPuntos(
            string mensaje);

        public event DelegadoPuntos?
            PuntosAcumulados;

        public void Disparar(
            string cliente,
            int puntos)
        {
            PuntosAcumulados?.Invoke(
                $"Cliente {cliente} acumuló {puntos} puntos");
        }
    }
}