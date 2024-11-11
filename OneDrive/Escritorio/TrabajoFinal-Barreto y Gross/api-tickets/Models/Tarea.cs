using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class Tarea
{
    public int Id { get; set; }
    public string? Titulo { get; set; }
    public string? Descripcion { get; set; }

    public int EstadoId { get; set; }

    public DateTime Fecha_modificacion { get; set; }

    public string? UsuarioId { get; set; }  // Clave foránea hacia Usuario (creador de la tarea)

    public string? TecnicoId { get; set; }  // Clave foránea hacia Usuario (técnico asignado)

    // Relación con los comentarios (1 tarea puede tener muchos comentarios)
    [JsonIgnore]
    public List<Comentario>? Comentarios { get; set; }

    [JsonIgnore]
    public List<CambioEstado>? CambiosEstado {get;set;}
}
