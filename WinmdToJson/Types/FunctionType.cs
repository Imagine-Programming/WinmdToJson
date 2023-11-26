namespace Win32MetadataJsonGen.Types;

internal class FunctionType(string name, string? namespaceName)
    : BaseType(name, namespaceName)
{
    public Reference<BaseType>? ReturnType { get; set; }
    public List<FunctionParameter>? Parameters { get; set; }
    public List<string>? MethodAttributes { get; set; }
    public List<string>? ImportAttributes { get; set; }
    public List<string>? ImplAttributes { get; set; }
}
