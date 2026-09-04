namespace BibFarmacia.Servicios
{
    public abstract class CargadorTxt<T>
    {
        public string Cargar(
            string ruta,
            ICollection<T> destino)
        {
            try
            {
                if (!File.Exists(ruta))
                {
                    return "Archivo no encontrado";
                }

                string[] lineas =
                    File.ReadAllLines(ruta);

                foreach (string linea in lineas)
                {
                    string[] campos =
                        linea.Split(';');

                    T elemento =
                        ParsearCampos(campos);

                    destino.Add(elemento);
                }

                return MensajeCargaExitosa;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        protected abstract T ParsearCampos(
            string[] campos);

        protected abstract string
            MensajeCargaExitosa { get; }
    }
}
