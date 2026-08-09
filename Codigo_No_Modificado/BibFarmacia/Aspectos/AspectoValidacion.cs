using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BibFarmacia.Clases;

namespace BibFarmacia.Aspectos
{
    public static class AspectoValidacion
    {
        public static string ValidarCliente(
            Cliente cliente)
        {
            if (string.IsNullOrWhiteSpace(
                cliente.Nombre))
            {
                return "Nombre inválido";
            }

            if (cliente.Cedula.Length < 3)
            {
                return "Cédula inválida";
            }

            return "Cliente válido";
        }

        public static string ValidarProducto(
            Producto producto)
        {
            if (producto.Precio <= 0)
            {
                return "Precio inválido";
            }

            if (producto.Stock < 0)
            {
                return "Stock inválido";
            }

            return "Producto válido";
        }
    }
}