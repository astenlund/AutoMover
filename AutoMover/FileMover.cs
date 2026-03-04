namespace AutoMover;

public class FileMover(IFileSystem fileSystem, IErrorReporter errorReporter)
{
    public bool TryGetSourcePath(string[] args, out string source)
    {
        source = string.Empty;

        if (args.Length != 1)
        {
            errorReporter.ShowError("Unexpected number of arguments.");
            return false;
        }

        source = args[0].Trim();

        if (!fileSystem.FileExists(source))
        {
            errorReporter.ShowError("Source file could not be found: " + source);
            return false;
        }

        return true;
    }

    public bool TryGetTargetPath(out string target, out bool overwrite, string source, AppSettings appSettings)
    {
        target = string.Empty;
        overwrite = false;

        var filename = Path.GetFileName(source);
        var extension = Path.GetExtension(filename);

        if (string.IsNullOrEmpty(extension))
        {
            errorReporter.ShowError("Unable to extract extension from source filename: " + filename);
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
            errorReporter.ShowError("No target directory configured for extension '" + extensionKey + "'");
            return false;
        }

        if (!fileSystem.DirectoryExists(targetOptions.Directory))
        {
            errorReporter.ShowError("Target directory could not be found: " + targetOptions.Directory);
            return false;
        }

        overwrite = targetOptions.Overwrite;
        target = Path.Combine(targetOptions.Directory, filename);

        return true;
    }

    public bool TryMove(string source, string target, bool overwrite)
    {
        try
        {
            if (!overwrite && fileSystem.FileExists(target))
            {
                if (!errorReporter.AskConfirmation("File already exists: " + target + "\n\nDo you want to replace it?"))
                    return false;

                overwrite = true;
            }

            fileSystem.MoveFile(source, target, overwrite);
            return true;
        }
        catch (Exception ex)
        {
            errorReporter.ShowError("Failed to move file: " + ex.Message);
            return false;
        }
    }
}
