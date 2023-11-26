namespace Win32MetadataJsonGen.Types;

internal class Field(string name)
{
    public Field(string name, Reference<BaseType> type)
        : this(name)
    {
        Type = type;
    }

    public string Name { get; set; } = name;
    public string? Value { get; set; }
    public Reference<BaseType>? Type { get; set; }
    public List<string>? Attributes { get; set; }
    public List<CustomAttribute>? CustomAttributes { get; set; }
}
