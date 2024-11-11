public class CambioEstado
{
    public int Id { get; set; }
    public int TareaId { get; set; } // ID de la tarea a la que pertenece el cambio de estado
    public int EstadoAnteriorId { get; set; } // El estado anterior antes del cambio
    public int EstadoNuevoId { get; set; } // El estado después del cambio
    public DateTime FechaCambio { get; set; } // Fecha en que se realizó el cambio
    public string UsuarioId { get; set; } // Usuario que realizó el cambio
}
