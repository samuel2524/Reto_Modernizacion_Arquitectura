using BibFarmacia.Clases;
using BibFarmacia.Interfaces;

namespace BibFarmacia.Servicios
{
    public class MonitorCompuesto :
        IReglaAlerta
    {
        private readonly List<IReglaAlerta>
            reglas;

        public MonitorCompuesto()
        {
            reglas = new List<IReglaAlerta>();
        }

        public void Agregar(
            IReglaAlerta regla)
        {
            reglas.Add(regla);
        }

        public void Verificar(
            IEnumerable<Producto> productos)
        {
            foreach (IReglaAlerta regla in reglas)
            {
                regla.Verificar(productos);
            }
        }
    }
}
