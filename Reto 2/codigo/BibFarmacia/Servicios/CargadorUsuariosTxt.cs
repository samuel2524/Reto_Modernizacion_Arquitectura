using BibFarmacia.Clases;
using BibFarmacia.Interfaces;

namespace BibFarmacia.Servicios
{
    public class CargadorUsuariosTxt :
        CargadorTxt<Usuario>,
        ICargadorUsuarios
    {
        protected override Usuario ParsearCampos(
            string[] campos)
        {
            return new Usuario(
                campos[0],
                campos[1],
                campos[2],
                campos[3],
                campos[4],
                campos[5]);
        }

        protected override string
            MensajeCargaExitosa =>
                "Usuarios cargados";
    }
}
