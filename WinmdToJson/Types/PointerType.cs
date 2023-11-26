namespace Win32MetadataJsonGen.Types;

internal class PointerType(string name, string? namespaceName)
    : BaseType(name, namespaceName)
{
    public bool IsPointer { get; set; } = true;
}
