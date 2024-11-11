
using System.Text.Json;

public class ComentarioFileService
{
    private readonly string _filePath = "Data/comentarios.json";
    private readonly IFileStorageService _fileStorageService;

    public ComentarioFileService(IFileStorageService fileStorageService)
    {
      _fileStorageService = fileStorageService;
    }
    public Comentario Create(Comentario a)
    {
        // Leer el contenido del archivo JSON
        var json = _fileStorageService.Read(_filePath);
        // Deserializar el JSON en una lista de comentarios
        var comentarios = JsonSerializer.Deserialize<List<Comentario>>(json) ?? new List<Comentario>();
        // Agregar el nuevo Comentario a la lista
        comentarios.Add(a);
        // Serializar la lista actualizada de vuelta a JSON
        json = JsonSerializer.Serialize(comentarios);
        // Escribir el JSON actualizado en el archivo
        _fileStorageService.Write(_filePath, json);
        return a;
    }

    public void Delete(int id)
    {
        // Leer el contenido del archivo JSON
        var json = _fileStorageService.Read(_filePath);
        // Deserializar el JSON en una lista de comentarios
        var comentarios = JsonSerializer.Deserialize<List<Comentario>>(json) ?? new List<Comentario>();
        // Buscar el Comentario por id
        var Comentario = comentarios.Find(Comentario => Comentario.Id == id);

        // Si el Comentario existe, eliminarlo de la lista
        if (Comentario is not null) 
        {
            comentarios.Remove(Comentario);
            // Serializar la lista actualizada de vuelta a JSON
            json = JsonSerializer.Serialize(comentarios);
            // Escribir el JSON actualizado en el archivo
            _fileStorageService.Write(_filePath, json);
        }
    }

    public IEnumerable<Comentario> GetAll()
    {
        //Leo el contenido del archivo
        var json = _fileStorageService.Read(_filePath);
        //Deserializo el Json en una lista de comentarios si es nulo retorna una lista vacia
        return JsonSerializer.Deserialize<List<Comentario>>(json) ?? new List<Comentario>();
    }

    public Comentario? GetById(int id)
    {
        //Leo el contenido del archivo
        var json = _fileStorageService.Read(_filePath);
        //Deserializo el Json en una lista de comentarios
        List<Comentario>? comentarios = JsonSerializer.Deserialize<List<Comentario>>(json);
        if(comentarios is null) return null;
        //Busco el Comentario por Id y devuelvo el Comentario encontrado
        return comentarios.Find(a => a.Id == id);  

    }

    public Comentario? Update(int id, Comentario Comentario)
    {
         // Leer el contenido del archivo JSON
        var json = _fileStorageService.Read(_filePath);
        // Deserializar el JSON en una lista de comentarios
        var comentarios = JsonSerializer.Deserialize<List<Comentario>>(json) ?? new List<Comentario>();
        // Buscar el índice del Comentario por id
        var ComentarioIndex = comentarios.FindIndex(a => a.Id == id);

        // Si el Comentario existe, reemplazarlo en la lista
        if (ComentarioIndex >= 0) 
        {
            //reeplazo el Comentario de la lista por el Comentario recibido por parametro con los nuevos datos
            comentarios[ComentarioIndex] = Comentario;
            // Serializar la lista actualizada de vuelta a JSON
            json = JsonSerializer.Serialize(comentarios);
            // Escribir el JSON actualizado en el archivo
            _fileStorageService.Write(_filePath, json);
            return Comentario;
        }

        // Retornar null si el Comentario no fue encontrado
        return null;
    }
    
}