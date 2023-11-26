namespace Win32MetadataJsonGen.Types;

internal class CustomAttribute(string name)
{
    public string Name { get; set; } = name;
    public List<object?>? FixedParameters { get; set; }
    public List<KeyValuePair<string, object?>>? NamedParameters { get; set; }
}
