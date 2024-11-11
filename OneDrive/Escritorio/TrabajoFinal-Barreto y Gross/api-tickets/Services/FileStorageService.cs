public class FileStorageService : IFileStorageService
{
    public string Read(string filePath)
    {
        if(!File.Exists(filePath))
        {
          File.WriteAllText(filePath,"[]");
        }

        return File.ReadAllText(filePath);
    }

    public void Write(string filePath, string fileContent)
    {
        File.WriteAllText(filePath, fileContent);
    }
}

public interface IFileStorageService
{
  string Read(string filePath);
  void Write(string filePath, string fileContent);
}