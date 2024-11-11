using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/Tareas")]
public class TareaController : ControllerBase
{
    private readonly ITareaService _tareaService;

    public TareaController(ITareaService tareaService, UserManager<ApplicationUser> userManager)
    {
        _tareaService = tareaService;

    }

    [HttpGet]
    //[AllowAnonymous]
    public ActionResult<List<Tarea>> GetAllTareas()
    {
        return Ok(_tareaService.GetAll());
    }

    [HttpGet("{id}/usuarios")]
    public ActionResult<Tarea> GetById(int id)
    {
        Tarea? a = _tareaService.GetById(id);
        if (a == null)
        {
            return NotFound("Tarea no Encotrado");
        }

        return Ok(a);

    }


    [HttpPost]
    public ActionResult<Tarea> NuevaTarea(Tarea t)
    {

        Tarea _t = _tareaService.Create(t);
        //Devuelvo el resultado de llamar al metodo GetById pasando como parametro el Id del nuevo Tarea
        return CreatedAtAction(nameof(GetById), new { id = _t.Id }, _t);
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var a = _tareaService.GetById(id);

        if (a == null)
        { return NotFound("Tarea no encontrado!!!"); }

        _tareaService.Delete(id);
        return NoContent();
    }

    [HttpPut("{id}")]
    public ActionResult<Tarea> UpdateTarea(int id, Tarea updatedTarea)
    {
        // Asegurarse de que el ID del Tarea en la solicitud coincida con el ID del parámetro
        if (id != updatedTarea.Id)
        {
            return BadRequest("El ID del Tarea en la URL no coincide con el ID del Tarea en el cuerpo de la solicitud.");
        }
        var Tarea = _tareaService.Update(id, updatedTarea);

        if (Tarea is null)
        {
            return NotFound(); // Si no se encontró el Tarea, retorna 404 Not Found
        }
        return CreatedAtAction(nameof(GetById), new { id = Tarea.Id }, Tarea); // Retorna el recurso actualizado
    }

    [HttpGet("tareas/estado/{estadoId}")]
    public IActionResult GetTareasByEstadoId(int estadoId)
    {
        var tareas = _tareaService.GetTareasByEstadoId(estadoId);
        if (tareas == null || !tareas.Any())
        {
            return NotFound("No se encontraron tareas para el estado especificado.");
        }

        return Ok(tareas);
    }
}