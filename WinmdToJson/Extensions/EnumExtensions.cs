namespace Win32MetadataJsonGen.Extensions;
internal static class EnumExtensions
{
    public static IEnumerable<Enum> GetFlags(this Enum input)
    {
        foreach (Enum value in Enum.GetValues(input.GetType()))
            if (input.HasFlag(value))
                yield return value;
    }

    public static IEnumerable<string>? GetFlagsAsStrings(this Enum input)
    {
        var strings = input.GetFlags()
            .Select(f => f.ToString())
            .Distinct();

        if (strings.Any())
            return strings;

        return null;
    }
}
