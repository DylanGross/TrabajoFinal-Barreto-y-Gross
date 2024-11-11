using Microsoft.AspNetCore.Http; // Para IHttpContextAccessor
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

public class ComentarioDbService : IComentarioService
{
    private readonly TicketsDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ComentarioDbService(TicketsDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public Comentario Create(ComentarioDTO c)
    {
        // Obtener el ID del usuario autenticado desde los Claims (JWT)
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException("El usuario no está autenticado.");
        }

        // Verificar que la tarea existe
        var tarea = _context.Tareas.Include(t => t.Comentarios).FirstOrDefault(t => t.Id == c.TareaId);
        if (tarea == null)
        {
            throw new ArgumentException("La tarea especificada no existe.");
        }

        // Verificar que el usuario autenticado sea el Técnico o el Usuario de la tarea
        if (tarea.TecnicoId.ToString() != userId && tarea.UsuarioId != userId)
        {
            throw new UnauthorizedAccessException("El usuario no está autorizado para comentar esta tarea.");
        }

        // Crear el comentario con los datos proporcionados
        Comentario comentario = new()
        {
            Descripcion = c.Descripcion,
            Fecha_comentario = DateTime.Now,
            UsuarioId = userId,
            TareaId = c.TareaId
        };

        // Agregar el comentario a la lista de comentarios de la tarea
        tarea.Comentarios.Add(comentario);

        // Agregar el comentario al contexto y guardar los cambios
        _context.Comentarios.Add(comentario);
        _context.SaveChanges();

        return comentario;
    }


    public void Delete(int id)
    {
        var comentarioExistente = _context.Comentarios.Find(id);
        if (comentarioExistente == null)
        {
            throw new ArgumentException("Comentario no encontrado.");
        }

        // Verificar que el usuario autenticado sea el mismo que el que creó el comentario
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (comentarioExistente.UsuarioId != userId)
        {
            throw new UnauthorizedAccessException("El usuario solo puede eliminar sus propios comentarios.");
        }

        // Eliminar el comentario
        _context.Comentarios.Remove(comentarioExistente);
        _context.SaveChanges();
    }


    public IEnumerable<Comentario> GetAll()
    {
        return _context.Comentarios;
    }

    public Comentario? GetById(int id)
    {
        return _context.Comentarios.Find(id);
    }

    public IEnumerable<Comentario> GetByTareaId(int tareaId)
    {
        return _context.Comentarios.Where(c => c.TareaId == tareaId).ToList();
    }

    public Comentario? Update(int id, Comentario c)
    {
        var comentarioExistente = _context.Comentarios.Find(id);
        if (comentarioExistente == null)
        {
            throw new ArgumentException("Comentario no encontrado.");
        }

        // Verificar que el usuario autenticado sea el mismo que el que creó el comentario
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (comentarioExistente.UsuarioId != userId)
        {
            throw new UnauthorizedAccessException("El usuario solo puede modificar sus propios comentarios.");
        }

        // Actualizar el comentario con los nuevos datos
        comentarioExistente.Descripcion = c.Descripcion;
        _context.Entry(comentarioExistente).State = EntityState.Modified;
        _context.SaveChanges();

        return comentarioExistente;
    }


}
