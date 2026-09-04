namespace BibFarmacia.Clases
{
    public class SolicitudConvenio
    {
        public decimal Subtotal { get; }
        public decimal Porcentaje { get; }
        public decimal CupoDisponible { get; }

        public SolicitudConvenio(
            decimal subtotal,
            decimal porcentaje,
            decimal cupoDisponible)
        {
            Subtotal = subtotal;
            Porcentaje = porcentaje;
            CupoDisponible = cupoDisponible;
        }
    }
}
