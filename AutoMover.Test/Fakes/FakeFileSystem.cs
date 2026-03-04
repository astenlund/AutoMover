namespace AutoMover.Test.Fakes;

public class FakeFileSystem : IFileSystem
{
    public HashSet<string> ExistingFiles { get; } = [];
    public HashSet<string> ExistingDirectories { get; } = [];
    public List<(string Source, string Destination, bool Overwrite)> MovedFiles { get; } = [];
    public Exception? MoveException { get; set; }

    public bool FileExists(string path) => ExistingFiles.Contains(path);
    public bool DirectoryExists(string path) => ExistingDirectories.Contains(path);

    public void MoveFile(string source, string destination, bool overwrite)
    {
        if (MoveException is not null)
            throw MoveException;

        MovedFiles.Add((source, destination, overwrite));
    }
}
