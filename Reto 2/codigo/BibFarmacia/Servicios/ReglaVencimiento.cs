using BibFarmacia.Clases;
using BibFarmacia.Eventos;
using BibFarmacia.Interfaces;

namespace BibFarmacia.Servicios
{
    public class ReglaVencimiento :
        IReglaAlerta
    {
        public EventoVencimiento EventoVencimiento;

        public ReglaVencimiento()
        {
            EventoVencimiento =
                new EventoVencimiento();
        }

        public void Verificar(
            IEnumerable<Producto> productos)
        {
            foreach (var producto in productos)
            {
                int dias =
                    (producto.FechaVencimiento -
                    DateTime.Now).Days;

                if (dias <= 30)
                {
                    EventoVencimiento
                        .Disparar(producto);
                }
            }
        }
    }
}
