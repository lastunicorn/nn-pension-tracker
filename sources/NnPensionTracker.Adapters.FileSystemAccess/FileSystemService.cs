using DustInTheWind.NnPensionTracker.Ports.FileSystemAccess;

namespace DustInTheWind.NnPensionTracker.Adapters.FileSystemAccess;

public class FileSystemService : IFileSystemService
{
    public string ReadAllText(string filePath)
    {
        if (filePath == null) throw new ArgumentNullException(nameof(filePath));

        return File.ReadAllText(filePath);
    }

    public IEnumerable<string> ReadTextLines(string filePath)
    {
        if (filePath == null) throw new ArgumentNullException(nameof(filePath));

        StreamReader reader = new(filePath);

        while (!reader.EndOfStream)
        {
            yield return reader.ReadLine();
        }
    }

    public StreamReader OpenStreamReader(string filePath)
    {
        if (filePath == null) throw new ArgumentNullException(nameof(filePath));

        return new StreamReader(filePath);
    }

    public IEnumerable<string> GetFiles(string directoryPath, string searchPattern)
    {
        if (directoryPath == null) throw new ArgumentNullException(nameof(directoryPath));

        return Directory.GetFiles(directoryPath, searchPattern);
    }

    public bool IsDirectory(string path)
    {
        if (path == null) throw new ArgumentNullException(nameof(path));

        return Directory.Exists(path);
    }

    public StreamWriter OpenStreamWriter(string filePath)
    {
        return new StreamWriter(filePath);
    }
}