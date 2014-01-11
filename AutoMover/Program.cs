namespace AutoMover
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;

    /// <summary>
    /// This class moves .torrent files from one directory to another.
    /// </summary>
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            string source, target;

            if (!GetSourcePath(args, out source))
            {
                Environment.Exit(1);
            }

            if (!GetTargetPath(out target, source))
            {
                Environment.Exit(1);
            }

            File.Move(source, target);
        }

        private static bool GetSourcePath(string[] args, out string source)
        {
            source = string.Empty;

            Environment.CurrentDirectory = Path.GetDirectoryName(Application.ExecutablePath) ?? string.Empty;

            if (args.Length != 1)
            {
                ErrorMessage("Unexpected number of arguments.");
                return false;
            }

            source = args[0].Trim();

            if (!File.Exists(source))
            {
                ErrorMessage("Source file could not be found: " + source);
                return false;
            }

            return true;
        }

        private static bool GetTargetPath(out string target, string source)
        {
            target = string.Empty;
            var targetConfigFile = Path.Combine(Environment.CurrentDirectory, "target.txt");

            if (!File.Exists(targetConfigFile))
            {
                ErrorMessage("Target config file not found: " + targetConfigFile);
                return false;
            }

            var targetDir = (File.ReadLines(targetConfigFile).FirstOrDefault() ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(targetDir))
            {
                ErrorMessage("Target config file does not point to a directory.");
                return false;
            }

            if (!Directory.Exists(targetDir))
            {
                ErrorMessage("Target directory could not be found: " + targetDir);
                return false;
            }

            if (!Path.IsPathRooted(targetDir))
            {
                targetDir = Path.Combine(Environment.CurrentDirectory, targetDir);
            }

            var filename = Path.GetFileName(source);

            if (filename == null)
            {
                ErrorMessage("Could not extract filename from source path: " + source);
                return false;
            }

            target = Path.Combine(targetDir, filename);

            return true;
        }

        private static void ErrorMessage(string msg)
        {
            MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
