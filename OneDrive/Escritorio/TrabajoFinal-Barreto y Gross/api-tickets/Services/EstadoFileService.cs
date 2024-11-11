
using System.Text.Json;

public class EstadoFileService
{
    private readonly string _filePath = "Data/estados.json";
    private readonly IFileStorageService _fileStorageService;

    public EstadoFileService(IFileStorageService fileStorageService)
    {
      _fileStorageService = fileStorageService;
    }

    public IEnumerable<Estado> GetAll()
    {
        //Leo el contenido del archivo
        var json = _fileStorageService.Read(_filePath);
        //Deserializo el Json en una lista de estados si es nulo retorna una lista vacia
        return JsonSerializer.Deserialize<List<Estado>>(json) ?? new List<Estado>();
    }

    public Estado? GetById(int id)
    {
        //Leo el contenido del archivo
        var json = _fileStorageService.Read(_filePath);
        //Deserializo el Json en una lista de estados
        List<Estado>? estados = JsonSerializer.Deserialize<List<Estado>>(json);
        if(estados is null) return null;
        //Busco el Estado por Id y devuelvo el Estado encontrado
        return estados.Find(a => a.Id == id);  

    }

    
}