namespace BibFarmacia.Interfaces
{
    public interface ISelectorCreadorProducto
    {
        ICreadorProducto Seleccionar(
            string tipo);
    }
}
