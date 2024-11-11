
public class EstadoDbService : IEstadoService
{
    private readonly TicketsDbContext _context;
    public EstadoDbService(TicketsDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Estado> GetAll()
    {
        return _context.Estados;
    }

    public Estado? GetById(int id)
    {
        return _context.Estados.Find(id);
    }
}