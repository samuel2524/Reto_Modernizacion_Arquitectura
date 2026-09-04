using BibFarmacia.Clases;
using BibFarmacia.Interfaces;

namespace BibFarmacia.Servicios
{
    public class CargadorProductosTxt :
        CargadorTxt<Producto>,
        ICargadorProductos
    {
        private readonly ISelectorCreadorProducto
            selector;

        public CargadorProductosTxt(
            ISelectorCreadorProducto selector)
        {
            this.selector = selector;
        }

        protected override Producto ParsearCampos(
            string[] campos)
        {
            DatosProducto datosProducto =
                new DatosProducto(
                    campos[0],
                    campos[1],
                    decimal.Parse(campos[2]),
                    int.Parse(campos[3]),
                    int.Parse(campos[4]),
                    DateTime.Parse(campos[5]),
                    campos[6..]);

            ICreadorProducto creador =
                selector.Seleccionar(
                    datosProducto.Tipo);

            return creador.Crear(
                datosProducto);
        }

        protected override string
            MensajeCargaExitosa =>
                "Productos cargados";
    }
}
