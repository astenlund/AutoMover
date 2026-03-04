namespace AutoMover.Test.Fakes;

public class FakeErrorReporter : IErrorReporter
{
    public List<string> Errors { get; } = [];

    public void ShowError(string message) => Errors.Add(message);
}
