namespace BibFarmacia.Clases
{
    public class Cosmetico : Producto
    {
        public string Marca { get; set; }
        public string Presentacion { get; set; }

        public Cosmetico(
            string nombre,
            decimal precio,
            int stock,
            int stockMinimo,
            DateTime fechaVencimiento,
            string marca,
            string presentacion)
            : base(nombre, precio, stock,
                  stockMinimo, fechaVencimiento)
        {
            Marca = marca;
            Presentacion = presentacion;
        }
    }
}
