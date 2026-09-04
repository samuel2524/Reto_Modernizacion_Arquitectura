using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BibFarmacia.Clases;
using BibFarmacia.Eventos;

namespace BibFarmacia.Servicios
{
    public class ServicioMovimiento
    {
        private List<Movimiento> movimientos;

        public EventoMovimiento EventoMovimiento;

        public ServicioMovimiento()
        {
            movimientos = new List<Movimiento>();

            EventoMovimiento =
                new EventoMovimiento();
        }

        public void RegistrarMovimiento(
            Movimiento movimiento)
        {
            movimientos.Add(movimiento);

            EventoMovimiento.Disparar(
                movimiento.Tipo);
        }

        public List<Movimiento>
            ObtenerMovimientos()
        {
            return movimientos;
        }
    }
}