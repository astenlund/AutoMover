namespace AutoMover.Test.Fakes;

public class FakeErrorReporter : IErrorReporter
{
    public List<string> Errors { get; } = [];
    public List<string> Confirmations { get; } = [];
    public bool ConfirmationResult { get; set; }

    public void ShowError(string message) => Errors.Add(message);

    public bool AskConfirmation(string message)
    {
        Confirmations.Add(message);
        return ConfirmationResult;
    }
}
