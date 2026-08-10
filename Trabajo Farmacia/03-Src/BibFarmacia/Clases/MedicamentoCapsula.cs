using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BibFarmacia.Enum;

namespace BibFarmacia.Clases
{
    public class MedicamentoCapsula : Medicamento
    {
        public TipoRelleno TipoRelleno { get; set; }

        public MedicamentoCapsula(
            string nombre,
            decimal precio,
            int stock,
            int stockMinimo,
            DateTime fechaVencimiento,
            Laboratorio laboratorio,
            TipoRelleno tipoRelleno)
            : base(nombre, precio, stock,
                  stockMinimo, fechaVencimiento,
                  laboratorio)
        {
            TipoRelleno = tipoRelleno;
        }
    }
}