public interface ITareaService
{
    public IEnumerable<Tarea> GetAll();
    public Tarea? GetById (int id);
    public Tarea Create (Tarea t);
    public void Delete (int id);
    public Tarea? Update (int id, Tarea t);
    IEnumerable<Tarea> GetTareasByEstadoId(int estadoId);
    IEnumerable<CambioEstado> GetCambiosEstadoByTareaId(int tareaId);  
    //public IEnumerable<Comentario> GetComentarios(int id);
    //public IEnumerable<Usuario> GetUsuarios(int id);
}