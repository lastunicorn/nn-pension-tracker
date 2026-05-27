namespace DustInTheWind.NnPensionTracker.Ports.FileSystemAccess;

public interface IFileSystemService
{
    string ReadAllText(string filePath);
    
    IEnumerable<string> ReadTextLines(string filePath);
    
    StreamReader OpenStreamReader(string filePath);
    
    IEnumerable<string> GetFiles(string directoryPath, string searchPattern);
    
    bool IsDirectory(string path);
    
    StreamWriter OpenStreamWriter(string filePath);
}