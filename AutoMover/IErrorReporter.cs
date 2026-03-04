namespace AutoMover;

public interface IErrorReporter
{
    void ShowError(string message);
    bool AskConfirmation(string message);
}

public class MessageBoxErrorReporter : IErrorReporter
{
    public void ShowError(string message)
    {
        System.Windows.Forms.MessageBox.Show(message, "Error",
            System.Windows.Forms.MessageBoxButtons.OK,
            System.Windows.Forms.MessageBoxIcon.Error);
    }

    public bool AskConfirmation(string message)
    {
        var result = System.Windows.Forms.MessageBox.Show(message, "Confirm",
            System.Windows.Forms.MessageBoxButtons.YesNo,
            System.Windows.Forms.MessageBoxIcon.Question);

        return result == System.Windows.Forms.DialogResult.Yes;
    }
}
