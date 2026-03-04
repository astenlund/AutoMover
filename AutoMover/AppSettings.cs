namespace AutoMover;

public class AppSettings
{
    public Dictionary<string, TargetOptions> Targets { get; init; } = new(StringComparer.OrdinalIgnoreCase);
}

public record TargetOptions
{
    required public string Directory { get; init; }
    public bool Overwrite { get; init; }
}
