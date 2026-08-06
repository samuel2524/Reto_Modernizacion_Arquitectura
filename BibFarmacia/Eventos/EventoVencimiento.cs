using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BibFarmacia.Clases;

namespace BibFarmacia.Eventos
{
    public class EventoVencimiento
    {
        public delegate void DelegadoVencimiento(
            string mensaje);

        public event DelegadoVencimiento?
            Vencimiento;

        public void Disparar(
            Producto producto)
        {
            Vencimiento?.Invoke(
                $"ALERTA: {producto.Nombre} próximo a vencer");
        }
    }
}