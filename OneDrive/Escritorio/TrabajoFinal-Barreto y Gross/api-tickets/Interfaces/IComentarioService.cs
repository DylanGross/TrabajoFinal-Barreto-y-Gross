public interface IComentarioService
{
    public IEnumerable<Comentario> GetAll();
    public Comentario? GetById (int id);
    public Comentario Create (ComentarioDTO c);
    public IEnumerable<Comentario> GetByTareaId(int tareaId);
    public void Delete (int id);
    public Comentario? Update (int id, Comentario c);
}