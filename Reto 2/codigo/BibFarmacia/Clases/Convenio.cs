using BibFarmacia.Enum;

namespace BibFarmacia.Clases
{
    public class Convenio
    {
        public string Entidad { get; }
        public TipoBeneficioConvenio TipoBeneficio { get; }
        public decimal Porcentaje { get; }
        public decimal CupoDisponible { get; set; }

        public Convenio(
            string entidad,
            TipoBeneficioConvenio tipoBeneficio,
            decimal porcentaje,
            decimal cupoDisponible)
        {
            Entidad = entidad;
            TipoBeneficio = tipoBeneficio;
            Porcentaje = porcentaje;
            CupoDisponible = cupoDisponible;
        }
    }
}
