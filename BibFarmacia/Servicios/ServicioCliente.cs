using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BibFarmacia.Clases;
using BibFarmacia.Interfaces;

namespace BibFarmacia.Servicios
{
    public class ServicioCliente
    {
        private List<Cliente> clientes;

        private readonly ICargadorClientes
            cargador;

        public ServicioCliente(
            ICargadorClientes cargador)
        {
            this.cargador = cargador;

            clientes = new List<Cliente>();
        }

        public void AgregarCliente(
            Cliente cliente)
        {
            clientes.Add(cliente);
        }

        public List<Cliente> ObtenerClientes()
        {
            return clientes;
        }

        public string Cargar(
            string ruta)
        {
            return cargador.Cargar(
                ruta,
                clientes);
        }
    }
}
