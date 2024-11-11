using Microsoft.AspNetCore.Http; // Para IHttpContextAccessor
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

public class TareaDbService : ITareaService
{
    private readonly TicketsDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TareaDbService(TicketsDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public Tarea Create(Tarea t)
    {
        // Obtener el ID del usuario autenticado desde los Claims
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException("El usuario no está autenticado.");
        }

        // Verificar que el técnico especificado exista
        var tecnico = _userManager.FindByIdAsync(t.TecnicoId.ToString()).Result; // Se utiliza .Result para hacerlo sincrónico
        if (tecnico == null)
        {
            throw new ArgumentException("El técnico especificado no existe.");
        }

        // Verificar si el técnico tiene el rol "tecnico"
        var roles = _userManager.GetRolesAsync(tecnico).Result; // Se utiliza .Result para hacerlo sincrónico
        if (!roles.Contains("Tecnico"))
        {
            throw new ArgumentException("El usuario especificado no tiene el rol 'tecnico'.");
        }

        // Crear la tarea con los datos proporcionados
        Tarea tarea = new()
        {
            Titulo = t.Titulo,
            Descripcion = t.Descripcion,
            EstadoId = t.EstadoId,
            TecnicoId = t.TecnicoId,  // Asegúrate de que este valor sea un ID válido de un usuario en AspNetUsers
            UsuarioId = userId,       // Asignar el ID del usuario autenticado
            Fecha_modificacion = t.Fecha_modificacion
        };

        // Agregar la tarea al contexto y guardar los cambios
        _context.Tareas.Add(tarea);
        _context.SaveChanges();

        return tarea;
    }

    public void Delete(int id)
    {
        var tarea = _context.Tareas.Find(id);
        if (tarea != null)
        {
            _context.Tareas.Remove(tarea);
            _context.SaveChanges();
        }
        else
        {
            throw new ArgumentException("Tarea no encontrada.");
        }
    }

    public IEnumerable<Tarea> GetAll()
    {
        return _context.Tareas;
    }

    public Tarea? GetById(int id)
    {
        // Cargar la tarea junto con sus comentarios asociados
        var tarea = _context.Tareas
            .Include(t => t.Comentarios)  // Incluir los comentarios relacionados con la tarea
            .FirstOrDefault(t => t.Id == id);  // Usar 'id' en lugar de 'tareaId'

        return tarea;
    }



    public Tarea Update(int id, Tarea t)
    {
        var tareaExistente = _context.Tareas.Find(id);
        if (tareaExistente == null)
        {
            throw new ArgumentException("Tarea no encontrada.");
        }

        // Obtener el ID del usuario autenticado
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException("El usuario no está autenticado.");
        }

        // Verificar que el usuario autenticado tenga el rol "Tecnico"
        var usuario = _userManager.FindByIdAsync(userId).Result;
        var roles = _userManager.GetRolesAsync(usuario).Result;
        if (!roles.Contains("Tecnico"))
        {
            throw new UnauthorizedAccessException("El usuario debe tener el rol 'Tecnico' para modificar la tarea.");
        }

        // Verificar que el técnico autenticado sea el mismo que el asignado a la tarea
        if (tareaExistente.TecnicoId.ToString() != userId)
        {
            throw new UnauthorizedAccessException("Solo el técnico asignado puede modificar esta tarea.");
        }

        // Registrar el cambio de estado si ha cambiado el estado
        if (t.EstadoId != tareaExistente.EstadoId)
        {
            var cambioEstado = new CambioEstado
            {
                TareaId = tareaExistente.Id,
                EstadoAnteriorId = tareaExistente.EstadoId,
                EstadoNuevoId = t.EstadoId,
                FechaCambio = DateTime.Now,
                UsuarioId = t.UsuarioId,
            };

            _context.CambiosEstado.Add(cambioEstado); // Registrar el cambio
        }

        // Actualizar la tarea con los nuevos datos
        tareaExistente.Titulo = t.Titulo;
        tareaExistente.Descripcion = t.Descripcion;
        tareaExistente.TecnicoId = userId;
        tareaExistente.Fecha_modificacion = t.Fecha_modificacion;
        tareaExistente.EstadoId = t.EstadoId; // Asegúrate de actualizar el estado

        _context.Entry(tareaExistente).State = EntityState.Modified;
        _context.SaveChanges();

        return tareaExistente;
    }


    public IEnumerable<Tarea> GetTareasByEstadoId(int estadoId)
    {
        // Asegúrate de que devuelves una lista de tareas
        var tareas = _context.Tareas
            .Where(t => t.EstadoId == estadoId)
            .Include(t => t.Comentarios) // Si necesitas comentarios asociados
            .ToList();  // Devuelve una lista de tareas

        return tareas;
    }


    // Método para obtener los cambios de estado de una tarea
    public IEnumerable<CambioEstado> GetCambiosEstadoByTareaId(int tareaId)
    {
        // Cargar los cambios de estado para la tarea sin intentar incluir la relación de EstadoAnterior
        var cambiosEstado = _context.CambiosEstado
            .Where(c => c.TareaId == tareaId)
            .ToList(); // Solo obtenemos los cambios de estado directamente

        // Devuelve los cambios de estado obtenidos
        return cambiosEstado;
    }




}
