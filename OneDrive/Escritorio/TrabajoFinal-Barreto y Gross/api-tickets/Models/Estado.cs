using System.Text.Json.Serialization;

public class Estado
{
    public int Id {get; set;}

    [JsonIgnore]
    public string? Tipo {get; set;}
    public int? TareaId {get; set;}
}