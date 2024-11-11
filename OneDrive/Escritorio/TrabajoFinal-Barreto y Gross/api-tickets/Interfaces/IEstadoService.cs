public interface IEstadoService
{
    public IEnumerable<Estado> GetAll();
    public Estado GetById (int id);
}