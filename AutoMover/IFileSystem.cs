namespace AutoMover;

public interface IFileSystem
{
    bool FileExists(string path);
    bool DirectoryExists(string path);
    void MoveFile(string source, string destination, bool overwrite);
}

public class FileSystem : IFileSystem
{
    public bool FileExists(string path) => File.Exists(path);
    public bool DirectoryExists(string path) => Directory.Exists(path);
    public void MoveFile(string source, string destination, bool overwrite) => File.Move(source, destination, overwrite);
}
