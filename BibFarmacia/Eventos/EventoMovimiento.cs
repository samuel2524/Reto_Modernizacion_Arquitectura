using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibFarmacia.Eventos
{
    public class EventoMovimiento
    {
        public delegate void DelegadoMovimiento(
            string mensaje);

        public event DelegadoMovimiento?
            MovimientoRegistrado;

        public void Disparar(
            string tipo)
        {
            MovimientoRegistrado?.Invoke(
                $"Movimiento registrado: {tipo}");
        }
    }
}