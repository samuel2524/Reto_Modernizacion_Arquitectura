using BibFarmacia.Interfaces;

namespace BibFarmacia.Factories
{
    public class SelectorCreadorProducto :
        ISelectorCreadorProducto
    {
        private readonly IDictionary<string,
            ICreadorProducto> creadores;

        public SelectorCreadorProducto(
            IDictionary<string,
                ICreadorProducto> creadores)
        {
            this.creadores = creadores;
        }

        public ICreadorProducto Seleccionar(
            string tipo)
        {
            return creadores[tipo];
        }
    }
}
