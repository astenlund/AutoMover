using Microsoft.Extensions.Configuration;

namespace AutoMover;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        Environment.CurrentDirectory = Path.GetDirectoryName(Application.ExecutablePath) ?? string.Empty;

        var fileSystem = new FileSystem();
        var errorReporter = new MessageBoxErrorReporter();
        var fileMover = new FileMover(fileSystem, errorReporter);

        if (!fileMover.TryGetSourcePath(args, out var source))
        {
            Environment.Exit(1);
        }

        IConfiguration config;

        try
        {
            config = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();
        }
        catch (FileNotFoundException)
        {
            var configPath = Path.Combine(Environment.CurrentDirectory, "appsettings.json");
            errorReporter.ShowError("Config file not found: " + configPath);
            Environment.Exit(1);

            return;
        }

        var appSettings = new AppSettings();
        config.Bind(appSettings);

        if (!fileMover.TryGetTargetPath(out var target, out var overwrite, source, appSettings))
        {
            Environment.Exit(1);
        }

        if (!fileMover.TryMove(source, target, overwrite))
        {
            Environment.Exit(1);
        }
    }
}
