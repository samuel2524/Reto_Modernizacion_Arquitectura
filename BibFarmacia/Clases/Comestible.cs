using BibFarmacia.Enum;

namespace BibFarmacia.Clases
{
    public class Comestible : Producto
    {
        public CategoriaComestible Categoria { get; set; }

        public Comestible(
            string nombre,
            decimal precio,
            int stock,
            int stockMinimo,
            DateTime fechaVencimiento,
            CategoriaComestible categoria)
            : base(nombre, precio, stock,
                  stockMinimo, fechaVencimiento)
        {
            Categoria = categoria;
        }
    }
}
