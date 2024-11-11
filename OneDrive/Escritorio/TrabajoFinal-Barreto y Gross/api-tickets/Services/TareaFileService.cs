
using System.Text.Json;

public class TareaFileService
{
    private readonly string _filePath = "Data/tareas.json";
    private readonly IFileStorageService _fileStorageService;

    public TareaFileService(IFileStorageService fileStorageService)
    {
      _fileStorageService = fileStorageService;
    }
    public Tarea Create(Tarea a)
    {
        // Leer el contenido del archivo JSON
        var json = _fileStorageService.Read(_filePath);
        // Deserializar el JSON en una lista de tareas
        var tareas = JsonSerializer.Deserialize<List<Tarea>>(json) ?? new List<Tarea>();
        // Agregar el nuevo Tarea a la lista
        tareas.Add(a);
        // Serializar la lista actualizada de vuelta a JSON
        json = JsonSerializer.Serialize(tareas);
        // Escribir el JSON actualizado en el archivo
        _fileStorageService.Write(_filePath, json);
        return a;
    }

    public void Delete(int id)
    {
        // Leer el contenido del archivo JSON
        var json = _fileStorageService.Read(_filePath);
        // Deserializar el JSON en una lista de tareas
        var tareas = JsonSerializer.Deserialize<List<Tarea>>(json) ?? new List<Tarea>();
        // Buscar el Tarea por id
        var Tarea = tareas.Find(Tarea => Tarea.Id == id);

        // Si el Tarea existe, eliminarlo de la lista
        if (Tarea is not null) 
        {
            tareas.Remove(Tarea);
            // Serializar la lista actualizada de vuelta a JSON
            json = JsonSerializer.Serialize(tareas);
            // Escribir el JSON actualizado en el archivo
            _fileStorageService.Write(_filePath, json);
        }
    }

    public IEnumerable<Tarea> GetAll()
    {
        //Leo el contenido del archivo
        var json = _fileStorageService.Read(_filePath);
        //Deserializo el Json en una lista de tareas si es nulo retorna una lista vacia
        return JsonSerializer.Deserialize<List<Tarea>>(json) ?? new List<Tarea>();
    }

    public Tarea? GetById(int id)
    {
        //Leo el contenido del archivo
        var json = _fileStorageService.Read(_filePath);
        //Deserializo el Json en una lista de tareas
        List<Tarea>? tareas = JsonSerializer.Deserialize<List<Tarea>>(json);
        if(tareas is null) return null;
        //Busco el Tarea por Id y devuelvo el Tarea encontrado
        return tareas.Find(a => a.Id == id);  

    }

    public Tarea? Update(int id, Tarea tarea)
    {
         // Leer el contenido del archivo JSON
        var json = _fileStorageService.Read(_filePath);
        // Deserializar el JSON en una lista de tareas
        var tareas = JsonSerializer.Deserialize<List<Tarea>>(json) ?? new List<Tarea>();
        // Buscar el índice del Tarea por id
        var tareaIndex = tareas.FindIndex(a => a.Id == id);

        // Si el Tarea existe, reemplazarlo en la lista
        if (tareaIndex >= 0) 
        {
            //reeplazo el Tarea de la lista por el Tarea recibido por parametro con los nuevos datos
            tareas[tareaIndex] = tarea;
            // Serializar la lista actualizada de vuelta a JSON
            json = JsonSerializer.Serialize(tareas);
            // Escribir el JSON actualizado en el archivo
            _fileStorageService.Write(_filePath, json);
            return tarea;
        }

        // Retornar null si el Tarea no fue encontrado
        return null;
    }
    
}