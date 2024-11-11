using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

[ApiController]
[Route("api/comentarios")]
public class ComentarioController : ControllerBase
{
    private readonly IComentarioService _comentarioService;

    public ComentarioController(IComentarioService comentarioService)
    {
        _comentarioService = comentarioService;
    }

    // Obtener todos los comentarios
    [HttpGet]
    public ActionResult<IEnumerable<Comentario>> GetAll()
    {
        return Ok(_comentarioService.GetAll());
    }

    // Obtener comentarios por ID de tarea
    [HttpGet("tarea/{tareaId}")]
    public ActionResult<IEnumerable<Comentario>> GetByTareaId(int tareaId)
    {
        var comentarios = _comentarioService.GetByTareaId(tareaId);
        if (!comentarios.Any())
        {
            return NotFound(new { Message = "No se encontraron comentarios para la tarea especificada" });
        }

        return Ok(comentarios);
    }

    // Obtener un comentario por ID
    [HttpGet("{id}")]
    public ActionResult<Comentario> GetById(int id)
    {
        var comentario = _comentarioService.GetById(id);
        if (comentario == null)
        {
            return NotFound(new { Message = "Comentario no encontrado" });
        }

        return Ok(comentario);
    }

    // Crear un nuevo comentario
    [HttpPost]
    public ActionResult<Comentario> Create([FromBody] ComentarioDTO comentarioDTO)
    {
        if (comentarioDTO == null)
        {
            return BadRequest(new { Message = "Datos de comentario inválidos" });
        }

        var comentarioCreado = _comentarioService.Create(comentarioDTO);
        return CreatedAtAction(nameof(GetById), new { id = comentarioCreado.Id }, comentarioCreado);
    }

    // Actualizar un comentario
    [HttpPut("{id}")]
    public ActionResult<Comentario> Update(int id, [FromBody] Comentario comentario)
    {
        if (id != comentario.Id)
        {
            return BadRequest(new { Message = "El ID del comentario en la URL no coincide con el ID en el cuerpo de la solicitud" });
        }

        var comentarioActualizado = _comentarioService.Update(id, comentario);
        if (comentarioActualizado == null)
        {
            return NotFound(new { Message = "Comentario no encontrado" });
        }

        return Ok(comentarioActualizado);
    }

    // Eliminar un comentario
    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var comentarioExistente = _comentarioService.GetById(id);
        if (comentarioExistente == null)
        {
            return NotFound(new { Message = "Comentario no encontrado" });
        }

        _comentarioService.Delete(id);
        return NoContent();
    }
}
