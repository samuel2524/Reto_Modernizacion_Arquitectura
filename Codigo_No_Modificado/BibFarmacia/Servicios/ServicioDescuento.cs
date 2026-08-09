using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BibFarmacia.Interfaces;

namespace BibFarmacia.Servicios
{
    public class ServicioDescuento : IDescuento
    {
        public decimal CalcularDescuento(decimal precio)
        {
            return precio * 0.10m;
        }
    }
}