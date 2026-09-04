using BibFarmacia.Clases;

namespace BibFarmacia.Interfaces
{
    public interface IReglaAlerta
    {
        void Verificar(
            IEnumerable<Producto> productos);
    }
}
