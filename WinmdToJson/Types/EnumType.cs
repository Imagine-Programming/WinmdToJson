namespace Win32MetadataJsonGen.Types;

internal class EnumType(string name, string? namespaceName, Reference<BaseType> type)
    : BaseType(name, namespaceName)
{
    public Reference<BaseType> Type { get; set; } = type;

    public List<string>? FieldAttributes { get; set; }
    public List<Field>? Fields { get; set; }
}
