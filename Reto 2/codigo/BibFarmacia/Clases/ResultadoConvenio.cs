namespace BibFarmacia.Clases
{
    public class ResultadoConvenio
    {
        public bool Aprobado { get; }
        public decimal Descuento { get; }
        public decimal Total { get; }
        public decimal CupoRestante { get; }
        public string? Motivo { get; }

        public ResultadoConvenio(
            bool aprobado,
            decimal descuento,
            decimal total,
            decimal cupoRestante,
            string? motivo)
        {
            Aprobado = aprobado;
            Descuento = descuento;
            Total = total;
            CupoRestante = cupoRestante;
            Motivo = motivo;
        }
    }
}
