
using System.Text.Json.Serialization;
using Win32MetadataJsonGen.Types;

namespace Win32MetadataJsonGen.Models;

internal class Namespace(string name)
{
    public string Name { get; set; } = name;
    public string Assembly { get; set; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<BaseType> Types { get; set; } = [];
}
