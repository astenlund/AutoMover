namespace AutoMover
{
    public static class Extensions
    {
        public static string RemoveLeading(this string str, string substring)
        {
            return str.StartsWith(substring) ? str.Substring(substring.Length, str.Length - substring.Length) : str;
        }

        public static string FormatFileExtension(this string ext)
        {
            return ext.RemoveLeading(".").ToUpperInvariant();
        }
    }
}
