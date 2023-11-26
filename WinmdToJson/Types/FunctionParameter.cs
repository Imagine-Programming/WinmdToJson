namespace Win32MetadataJsonGen.Types;

internal class FunctionParameter(string name, Reference<BaseType> type)
{
    public string Name { get; set; } = name;
    public string? Value { get; set; }
    public Reference<BaseType> Type { get; set; } = type;
    public List<string>? Attributes { get; set; }
    public List<CustomAttribute>? CustomAttributes { get; set; }
}
