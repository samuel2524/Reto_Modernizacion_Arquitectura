using BibFarmacia.Clases;
using BibFarmacia.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibFarmacia.Factories
{
    public static class ProductoFactory
    {
        public static MedicamentoCapsula CrearCapsula(
            string nombre,
            decimal precio,
            int stock,
            Laboratorio laboratorio)
        {
            return new MedicamentoCapsula(
                nombre,
                precio,
                stock,
                5,
                DateTime.Now.AddMonths(6),
                laboratorio,
                TipoRelleno.Gel);
        }
        public static MedicamentoLiquido CrearLiquido(
            string nombre,
            decimal precio,
            int stock,
            Laboratorio laboratorio)
        {
            return new MedicamentoLiquido(
                nombre,
                precio,
                stock,
                5,
                DateTime.Now.AddMonths(12),
                laboratorio,
                MaterialEnvase.Vidrio,
                120);
        }
    }
}