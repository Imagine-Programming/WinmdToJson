using System.Text.Json.Serialization;
using Win32MetadataJsonGen.Types;

namespace Win32MetadataJsonGen;

internal class Reference<T> where T : BaseType
{
    [JsonIgnore]
    public T? Value { get; set; }

    public string? Kind => Value == null ? throw new ArgumentNullException(nameof(Value)) : Value.GetType().Name;
    public string? Namespace => Value == null ? throw new ArgumentNullException(nameof(Value)) : Value.Namespace;
    public string Type => Value == null ? throw new ArgumentNullException(nameof(Value)) : Value.Name;
    
    // Array-specific properties
    public ArrayType? ArrayInformation => Value is ArrayType arrayType ? arrayType : null;
}
