using Microsoft.Extensions.Configuration;

using static System.Windows.Forms.MessageBoxButtons;
using static System.Windows.Forms.MessageBoxIcon;

namespace AutoMover;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        Environment.CurrentDirectory = Path.GetDirectoryName(Application.ExecutablePath) ?? string.Empty;

        if (args.Length != 1)
        {
            ErrorMessage("Unexpected number of arguments.");
            Environment.Exit(1);
        }

        if (!GetSourcePath(args, out var source))
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
            ErrorMessage("Config file not found: " + configPath);
            Environment.Exit(1);

            return;
        }

        var appSettings = new AppSettings();
        config.Bind(appSettings);

        if (!GetTargetPath(out var target, out var overwrite, source, appSettings))
        {
            Environment.Exit(1);
        }

        try
        {
            File.Move(source, target, overwrite);
        }
        catch (Exception ex)
        {
            ErrorMessage("Failed to move file: " + ex.Message);
            Environment.Exit(1);
        }
    }

    private static bool GetSourcePath(string[] args, out string source)
    {
        source = args[0].Trim();

        if (!File.Exists(source))
        {
            ErrorMessage("Source file could not be found: " + source);

            return false;
        }

        return true;
    }

    private static bool GetTargetPath(out string target, out bool overwrite, string source, AppSettings appSettings)
    {
        target = string.Empty;
        overwrite = false;

        var filename = Path.GetFileName(source);
        var extension = Path.GetExtension(filename);

        if (string.IsNullOrEmpty(extension))
        {
            ErrorMessage("Unable to extract extension from source filename: " + filename);

            return false;
        }

        var extensionKey = extension.RemoveLeading(".");

        if (!appSettings.Targets.TryGetValue(extensionKey, out var targetOptions))
        {
            extensionKey = extension;
            appSettings.Targets.TryGetValue(extensionKey, out targetOptions);
        }

        if (targetOptions is null || string.IsNullOrEmpty(targetOptions.Directory))
        {
            ErrorMessage("No target directory configured for extension '" + extensionKey + "'");

            return false;
        }

        if (!Directory.Exists(targetOptions.Directory))
        {
            ErrorMessage("Target directory could not be found: " + targetOptions.Directory);

            return false;
        }

        overwrite = targetOptions.Overwrite;
        target = Path.Combine(targetOptions.Directory, filename);

        return true;
    }

    private static void ErrorMessage(string msg)
    {
        MessageBox.Show(msg, "Error", OK, Error);
    }
}
