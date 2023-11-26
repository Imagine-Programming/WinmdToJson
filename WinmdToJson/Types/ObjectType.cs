namespace Win32MetadataJsonGen.Types;

internal class ObjectType(string name, string? namespaceName)
    : BaseType(name, namespaceName)
{
    public List<Field>? Fields { get; set; }
    public List<BaseType>? Types { get; set; }
    public List<FunctionType>? Methods { get; set; }
}
