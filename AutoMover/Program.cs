namespace AutoMover
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;

    /// <summary>
    /// This class moves files from one directory to another.
    /// </summary>
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Environment.CurrentDirectory = Path.GetDirectoryName(Application.ExecutablePath) ?? string.Empty;

            string source, target;

            if (args.Length != 1)
            {
                ErrorMessage("Unexpected number of arguments.");
                Environment.Exit(1);
            }

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

        private static bool GetSourcePath(IList<string> args, out string source)
        {
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

            var filename = Path.GetFileName(source);

            if (filename == null)
            {
                ErrorMessage("Could not extract filename from source path: " + source);
                return false;
            }

            var extension = Path.GetExtension(filename);

            if (string.IsNullOrEmpty(extension))
            {
                ErrorMessage("Unable to extract extension from source filename: " + filename);
                return false;
            }

            var targetDir = GetTargetDir(targetConfigFile, extension);

            if (string.IsNullOrEmpty(targetDir))
            {
                ErrorMessage("Syntax error in target config file. Location of file: " + targetConfigFile);
                return false;
            }

            if (!Directory.Exists(targetDir))
            {
                ErrorMessage("Target directory could not be found: " + targetDir);
                return false;
            }

            target = Path.Combine(targetDir, filename);

            return true;
        }

        private static string GetTargetDir(string targetConfigFile, string extension)
        {
            var targetConfig = ParseTargetConfig(targetConfigFile);
            
            return targetConfig[extension.FormatFileExtension()] ?? string.Empty;
        }

        private static Dictionary<string, string> ParseTargetConfig(string targetConfigFile)
        {
            var result = new Dictionary<string, string>();

            foreach (var tuple in File.ReadLines(targetConfigFile)
                                      .SkipWhile(s => s.StartsWith("#"))
                                      .Select(s => Array.ConvertAll(s.Split('='), input => input.Trim()))
                                      .TakeWhile(arr => arr.Length == 2)
                                      .Select(strings => new Tuple<string, string>(strings[0].FormatFileExtension(), strings[1])))
            {
                var ext = tuple.Item1;
                var dir = tuple.Item2;

                result[ext] = dir;
            }

            return result;
        }

        private static void ErrorMessage(string msg)
        {
            MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
