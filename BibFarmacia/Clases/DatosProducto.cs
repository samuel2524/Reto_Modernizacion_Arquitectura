namespace BibFarmacia.Clases
{
    public class DatosProducto
    {
        public string Tipo { get; }
        public string Nombre { get; }
        public decimal Precio { get; }
        public int Stock { get; }
        public int StockMinimo { get; }
        public DateTime FechaVencimiento { get; }
        public IReadOnlyList<string> Extra { get; }

        public DatosProducto(
            string tipo,
            string nombre,
            decimal precio,
            int stock,
            int stockMinimo,
            DateTime fechaVencimiento,
            IReadOnlyList<string> extra)
        {
            Tipo = tipo;
            Nombre = nombre;
            Precio = precio;
            Stock = stock;
            StockMinimo = stockMinimo;
            FechaVencimiento = fechaVencimiento;
            Extra = extra;
        }
    }
}
