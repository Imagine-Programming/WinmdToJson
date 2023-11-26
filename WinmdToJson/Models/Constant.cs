using System.Text.Json.Serialization;

namespace Win32MetadataJsonGen.Models;

internal class Constant(string name, Type type)
{
    public string Name { get; set; } = name;
    public Type Type { get; set; } = type;
    public string? Value { get; set; } = null;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<Attribute>? Attributes { get; set; }
}
