using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BibFarmacia.Enum;

namespace BibFarmacia.Clases
{
    public class MedicamentoLiquido : Medicamento
    {
        public MaterialEnvase MaterialEnvase { get; set; }
        public int Mililitros { get; set; }

        public MedicamentoLiquido(
            string nombre,
            decimal precio,
            int stock,
            int stockMinimo,
            DateTime fechaVencimiento,
            Laboratorio laboratorio,
            MaterialEnvase materialEnvase,
            int mililitros)
            : base(nombre, precio, stock,
                  stockMinimo, fechaVencimiento,
                  laboratorio)
        {
            MaterialEnvase = materialEnvase;
            Mililitros = mililitros;
        }
    }
}