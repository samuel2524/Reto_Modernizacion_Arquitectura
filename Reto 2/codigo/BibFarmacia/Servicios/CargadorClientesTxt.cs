using BibFarmacia.Clases;
using BibFarmacia.Enum;
using BibFarmacia.Interfaces;

namespace BibFarmacia.Servicios
{
    public class CargadorClientesTxt :
        CargadorTxt<Cliente>,
        ICargadorClientes
    {
        protected override Cliente ParsearCampos(
            string[] campos)
        {
            if (campos.Length == 4)
            {
                return new Cliente(
                    campos[0],
                    campos[1],
                    campos[2],
                    campos[3]);
            }

            if (campos.Length == 8)
            {
                TipoBeneficioConvenio beneficio =
                    System.Enum.Parse<TipoBeneficioConvenio>(
                        campos[5],
                        true);

                Convenio convenio =
                    new Convenio(
                        campos[4],
                        beneficio,
                        decimal.Parse(campos[6]),
                        decimal.Parse(campos[7]));

                return new Cliente(
                    campos[0],
                    campos[1],
                    campos[2],
                    campos[3],
                    convenio);
            }

            throw new FormatException(
                "Formato de cliente inválido");
        }

        protected override string
            MensajeCargaExitosa =>
                "Clientes cargados";
    }
}
