namespace AutoMover;

public interface IErrorReporter
{
    void ShowError(string message);
}

public class MessageBoxErrorReporter : IErrorReporter
{
    public void ShowError(string message)
    {
        System.Windows.Forms.MessageBox.Show(message, "Error",
            System.Windows.Forms.MessageBoxButtons.OK,
            System.Windows.Forms.MessageBoxIcon.Error);
    }
}
