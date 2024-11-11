public class Comentario
{
    public int Id { get; set; }
    public int TareaId {get;set;}
    public string? Descripcion { get; set; }
    public DateTime Fecha_comentario { get; set; }
    public string UsuarioId {get; set;}
    public Comentario()
    {

    }

    public Comentario(int id, string descripcion, string usuarioid, DateTime fecha_comentario)
    {
        Id = id;
        Descripcion = descripcion;
        UsuarioId = usuarioid;
        Fecha_comentario = fecha_comentario;
    }
}