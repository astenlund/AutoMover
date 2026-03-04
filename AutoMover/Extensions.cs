namespace AutoMover;

public static class Extensions
{
    extension(string str)
    {
        public string RemoveLeading(string substring)
        {
            return str.StartsWith(substring) ? str.Substring(substring.Length, str.Length - substring.Length) : str;
        }

        public string FormatFileExtension()
        {
            return str.RemoveLeading(".").ToUpperInvariant();
        }
    }
}
