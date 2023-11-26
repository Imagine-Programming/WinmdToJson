using System.Reflection.Metadata;

namespace Win32MetadataJsonGen.Types;

internal class ArrayType(string name, string? namespaceName, ArrayShape shape)
    : BaseType(name, namespaceName)
{
    public ArrayShape Shape { get; set; } = shape;
}