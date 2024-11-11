using System.ComponentModel.DataAnnotations;

public class TareaDTO
{
    [Required(ErrorMessage = "El campo Titulo es requerido.")]
    public string? Titulo { get; set; }
    [Required(ErrorMessage = "El campo Descripcion es requerido.")]
    public string? Descripcion { get; set; }
    public int? EstadoId{get; set;}
    public DateTime Fecha_modificacion { get; set; }

    public string? UsuarioId{get; set;}
    
    public string? TecnicoId {get; set;}

    
}