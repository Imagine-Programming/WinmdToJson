using System.Reflection.Metadata;
using System.Text.Json.Serialization;

namespace Win32MetadataJsonGen.Types;

internal class ArrayType(string name, string? namespaceName, ArrayShape shape)
    : BaseType(name, namespaceName)
{
    public ArrayShape Shape { get; set; } = shape;
    
    /// <summary>
    /// Reference to the element type of this array
    /// </summary>
    public Reference<BaseType>? ElementType { get; set; }
}