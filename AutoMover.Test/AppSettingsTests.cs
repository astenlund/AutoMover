using Microsoft.Extensions.Configuration;

namespace AutoMover.Test;

public class AppSettingsTests
{
    [Test]
    public void TargetsDictionaryIsCaseInsensitive()
    {
        var settings = new AppSettings();
        settings.Targets["TXT"] = new TargetOptions { Directory = "/tmp" };

        Assert.That(settings.Targets.ContainsKey("txt"), Is.True);
        Assert.That(settings.Targets.ContainsKey("TXT"), Is.True);
        Assert.That(settings.Targets.ContainsKey("Txt"), Is.True);
    }

    [Test]
    public void TargetsDictionaryDefaultsToEmpty()
    {
        var settings = new AppSettings();

        Assert.That(settings.Targets, Is.Empty);
    }

    [Test]
    public void TargetOptionsOverwriteDefaultsToFalse()
    {
        var options = new TargetOptions { Directory = "/tmp" };

        Assert.That(options.Overwrite, Is.False);
    }

    [Test]
    public void TargetOptionsStoresOverwrite()
    {
        var options = new TargetOptions { Directory = "/tmp", Overwrite = true };

        Assert.That(options.Overwrite, Is.True);
    }

    [Test]
    public void BindsFromConfiguration()
    {
        var configData = new Dictionary<string, string?>
        {
            ["Targets:txt:Directory"] = "/tmp/text",
            ["Targets:txt:Overwrite"] = "true",
            ["Targets:pdf:Directory"] = "/tmp/pdf",
        };

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        var settings = new AppSettings();
        config.Bind(settings);

        Assert.That(settings.Targets, Has.Count.EqualTo(2));
        Assert.That(settings.Targets["txt"].Directory, Is.EqualTo("/tmp/text"));
        Assert.That(settings.Targets["txt"].Overwrite, Is.True);
        Assert.That(settings.Targets["pdf"].Directory, Is.EqualTo("/tmp/pdf"));
        Assert.That(settings.Targets["pdf"].Overwrite, Is.False);
    }

    [Test]
    public void BoundTargetsAreCaseInsensitive()
    {
        var configData = new Dictionary<string, string?>
        {
            ["Targets:TXT:Directory"] = "/tmp/text",
        };

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        var settings = new AppSettings();
        config.Bind(settings);

        Assert.That(settings.Targets.ContainsKey("txt"), Is.True);
    }
}
