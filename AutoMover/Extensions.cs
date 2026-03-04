namespace AutoMover;

public static class Extensions
{
    extension(string str)
    {
        public string RemoveLeading(string substring)
        {
            return str.StartsWith(substring) ? str[substring.Length..] : str;
        }
    }
}
