using Win32MetadataJsonGen.Models;
using Win32MetadataJsonGen.Types;

namespace Win32MetadataJsonGen;

internal class ReadResult(List<Namespace> namespaces, Dictionary<string, Reference<BaseType>> types)
{
    public IReadOnlyList<Namespace> Namespaces { get; set; } = namespaces;
    public IReadOnlyDictionary<string, Reference<BaseType>> Types = types;
}
