using System.Reflection.Metadata;
using System.Text.Json.Serialization;

namespace Win32MetadataJsonGen.Types;

internal class ArrayType(string name, string? namespaceName, ArrayShape shape)
    : BaseType(name, namespaceName)
{
    [JsonIgnore]
    public ArrayShape Shape { get; set; } = shape;
    
    /// <summary>
    /// Reference to the element type of this array
    /// </summary>
    public Reference<BaseType>? ElementType { get; set; }
    
    /// <summary>
    /// Gets the number of dimensions in the array
    /// </summary>
    public int Rank => Shape.Rank;
    
    /// <summary>
    /// Gets the sizes of each dimension (if specified in metadata)
    /// </summary>
    public IReadOnlyList<int>? Sizes => Shape.Sizes.Length > 0 ? Shape.Sizes : null;
    
    /// <summary>
    /// Gets the lower bounds of each dimension (if specified in metadata)
    /// </summary>
    public IReadOnlyList<int>? LowerBounds => Shape.LowerBounds.Length > 0 ? Shape.LowerBounds : null;
}