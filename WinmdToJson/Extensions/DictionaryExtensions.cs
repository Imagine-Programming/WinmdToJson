namespace Win32MetadataJsonGen.Extensions;
internal static class DictionaryExtensions
{
    public static TValue GetOrCreate<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey name)
        where TKey : notnull
        where TValue : new()
    {
        if (!dictionary.TryGetValue(name, out var value))
        {
            value = new TValue();
            dictionary[name] = value;
        }

        return value;
    }
}
