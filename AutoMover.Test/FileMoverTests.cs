using AutoMover.Test.Fakes;

namespace AutoMover.Test;

public class FileMoverTests
{
    private FakeFileSystem _fileSystem = null!;
    private FakeErrorReporter _errorReporter = null!;
    private FileMover _fileMover = null!;

    [SetUp]
    public void SetUp()
    {
        _fileSystem = new FakeFileSystem();
        _errorReporter = new FakeErrorReporter();
        _fileMover = new FileMover(_fileSystem, _errorReporter);
    }

    #region TryGetSourcePath

    [Test]
    public void TryGetSourcePath_NoArgs_ReturnsFalseWithError()
    {
        var result = _fileMover.TryGetSourcePath([], out var source);

        Assert.That(result, Is.False);
        Assert.That(source, Is.EqualTo(string.Empty));
        Assert.That(_errorReporter.Errors, Has.Count.EqualTo(1));
        Assert.That(_errorReporter.Errors[0], Does.Contain("Unexpected number of arguments"));
    }

    [Test]
    public void TryGetSourcePath_TooManyArgs_ReturnsFalseWithError()
    {
        var result = _fileMover.TryGetSourcePath(["a", "b"], out _);

        Assert.That(result, Is.False);
        Assert.That(_errorReporter.Errors[0], Does.Contain("Unexpected number of arguments"));
    }

    [Test]
    public void TryGetSourcePath_FileDoesNotExist_ReturnsFalseWithError()
    {
        var result = _fileMover.TryGetSourcePath(["/missing/file.txt"], out _);

        Assert.That(result, Is.False);
        Assert.That(_errorReporter.Errors[0], Does.Contain("Source file could not be found"));
    }

    [Test]
    public void TryGetSourcePath_FileExists_ReturnsTrueWithPath()
    {
        _fileSystem.ExistingFiles.Add("/some/file.txt");

        var result = _fileMover.TryGetSourcePath(["/some/file.txt"], out var source);

        Assert.That(result, Is.True);
        Assert.That(source, Is.EqualTo("/some/file.txt"));
        Assert.That(_errorReporter.Errors, Is.Empty);
    }

    [Test]
    public void TryGetSourcePath_TrimsWhitespace()
    {
        _fileSystem.ExistingFiles.Add("/some/file.txt");

        var result = _fileMover.TryGetSourcePath(["  /some/file.txt  "], out var source);

        Assert.That(result, Is.True);
        Assert.That(source, Is.EqualTo("/some/file.txt"));
    }

    #endregion

    #region TryGetTargetPath

    [Test]
    public void TryGetTargetPath_NoExtension_ReturnsFalseWithError()
    {
        var settings = new AppSettings();

        var result = _fileMover.TryGetTargetPath(out _, out _, "/some/Makefile", settings);

        Assert.That(result, Is.False);
        Assert.That(_errorReporter.Errors[0], Does.Contain("Unable to extract extension"));
    }

    [Test]
    public void TryGetTargetPath_ExtensionNotConfigured_ReturnsFalseWithError()
    {
        var settings = new AppSettings();

        var result = _fileMover.TryGetTargetPath(out _, out _, "/some/file.xyz", settings);

        Assert.That(result, Is.False);
        Assert.That(_errorReporter.Errors[0], Does.Contain("No target directory configured for extension"));
    }

    [Test]
    public void TryGetTargetPath_MatchesExtensionWithoutDot()
    {
        _fileSystem.ExistingDirectories.Add("/target");
        var settings = new AppSettings();
        settings.Targets["txt"] = new TargetOptions { Directory = "/target" };

        var result = _fileMover.TryGetTargetPath(out var target, out _, "/some/file.txt", settings);

        Assert.That(result, Is.True);
        Assert.That(target, Does.EndWith("file.txt"));
        Assert.That(target, Does.StartWith("/target"));
    }

    [Test]
    public void TryGetTargetPath_MatchesExtensionWithDot()
    {
        _fileSystem.ExistingDirectories.Add("/target");
        var settings = new AppSettings();
        settings.Targets[".txt"] = new TargetOptions { Directory = "/target" };

        var result = _fileMover.TryGetTargetPath(out var target, out _, "/some/file.txt", settings);

        Assert.That(result, Is.True);
        Assert.That(target, Does.EndWith("file.txt"));
    }

    [Test]
    public void TryGetTargetPath_PrefersExtensionWithoutDot()
    {
        _fileSystem.ExistingDirectories.Add("/target-no-dot");
        _fileSystem.ExistingDirectories.Add("/target-with-dot");
        var settings = new AppSettings();
        settings.Targets["txt"] = new TargetOptions { Directory = "/target-no-dot" };
        settings.Targets[".txt"] = new TargetOptions { Directory = "/target-with-dot" };

        var result = _fileMover.TryGetTargetPath(out var target, out _, "/some/file.txt", settings);

        Assert.That(result, Is.True);
        Assert.That(target, Does.StartWith("/target-no-dot"));
    }

    [Test]
    public void TryGetTargetPath_TargetDirectoryDoesNotExist_ReturnsFalseWithError()
    {
        var settings = new AppSettings();
        settings.Targets["txt"] = new TargetOptions { Directory = "/nonexistent" };

        var result = _fileMover.TryGetTargetPath(out _, out _, "/some/file.txt", settings);

        Assert.That(result, Is.False);
        Assert.That(_errorReporter.Errors[0], Does.Contain("Target directory could not be found"));
    }

    [Test]
    public void TryGetTargetPath_SetsOverwriteFromOptions()
    {
        _fileSystem.ExistingDirectories.Add("/target");
        var settings = new AppSettings();
        settings.Targets["txt"] = new TargetOptions { Directory = "/target", Overwrite = true };

        _fileMover.TryGetTargetPath(out _, out var overwrite, "/some/file.txt", settings);

        Assert.That(overwrite, Is.True);
    }

    [Test]
    public void TryGetTargetPath_OverwriteDefaultsToFalse()
    {
        _fileSystem.ExistingDirectories.Add("/target");
        var settings = new AppSettings();
        settings.Targets["txt"] = new TargetOptions { Directory = "/target" };

        _fileMover.TryGetTargetPath(out _, out var overwrite, "/some/file.txt", settings);

        Assert.That(overwrite, Is.False);
    }

    [Test]
    public void TryGetTargetPath_CombinesDirectoryAndFilename()
    {
        _fileSystem.ExistingDirectories.Add("/my/target");
        var settings = new AppSettings();
        settings.Targets["pdf"] = new TargetOptions { Directory = "/my/target" };

        _fileMover.TryGetTargetPath(out var target, out _, "/downloads/report.pdf", settings);

        Assert.That(target, Is.EqualTo(Path.Combine("/my/target", "report.pdf")));
    }

    [Test]
    public void TryGetTargetPath_CaseInsensitiveExtensionLookup()
    {
        _fileSystem.ExistingDirectories.Add("/target");
        var settings = new AppSettings();
        settings.Targets["TXT"] = new TargetOptions { Directory = "/target" };

        var result = _fileMover.TryGetTargetPath(out var target, out _, "/some/file.txt", settings);

        Assert.That(result, Is.True);
    }

    [Test]
    public void TryGetTargetPath_EmptyDirectory_ReturnsFalseWithError()
    {
        var settings = new AppSettings();
        settings.Targets["txt"] = new TargetOptions { Directory = "" };

        var result = _fileMover.TryGetTargetPath(out _, out _, "/some/file.txt", settings);

        Assert.That(result, Is.False);
        Assert.That(_errorReporter.Errors[0], Does.Contain("No target directory configured"));
    }

    #endregion

    #region TryMove

    [Test]
    public void TryMove_Success_ReturnsTrueAndMovesFile()
    {
        var result = _fileMover.TryMove("/source/file.txt", "/target/file.txt", false);

        Assert.That(result, Is.True);
        Assert.That(_fileSystem.MovedFiles, Has.Count.EqualTo(1));
        Assert.That(_fileSystem.MovedFiles[0].Source, Is.EqualTo("/source/file.txt"));
        Assert.That(_fileSystem.MovedFiles[0].Destination, Is.EqualTo("/target/file.txt"));
        Assert.That(_fileSystem.MovedFiles[0].Overwrite, Is.False);
    }

    [Test]
    public void TryMove_WithOverwrite_PassesOverwriteFlag()
    {
        _fileMover.TryMove("/source/file.txt", "/target/file.txt", true);

        Assert.That(_fileSystem.MovedFiles[0].Overwrite, Is.True);
    }

    [Test]
    public void TryMove_Exception_ReturnsFalseWithError()
    {
        _fileSystem.MoveException = new IOException("Access denied");

        var result = _fileMover.TryMove("/source/file.txt", "/target/file.txt", false);

        Assert.That(result, Is.False);
        Assert.That(_errorReporter.Errors[0], Does.Contain("Failed to move file"));
        Assert.That(_errorReporter.Errors[0], Does.Contain("Access denied"));
    }

    [Test]
    public void TryMove_NoError_ReportsNoErrors()
    {
        _fileMover.TryMove("/source/file.txt", "/target/file.txt", false);

        Assert.That(_errorReporter.Errors, Is.Empty);
    }

    #endregion
}
