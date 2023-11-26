using System.Text.Json.Serialization;

namespace Win32MetadataJsonGen.Types;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "Kind")]
[JsonDerivedType(typeof(ArrayType), nameof(ArrayType))]
[JsonDerivedType(typeof(PointerType), nameof(PointerType))]
[JsonDerivedType(typeof(FunctionPointerType), nameof(FunctionPointerType))]
[JsonDerivedType(typeof(FunctionType), nameof(FunctionType))]
[JsonDerivedType(typeof(EnumType), nameof(EnumType))]
[JsonDerivedType(typeof(PrimitiveType), nameof(PrimitiveType))]
[JsonDerivedType(typeof(StructType), nameof(StructType))]
[JsonDerivedType(typeof(UnionType), nameof(UnionType))]
[JsonDerivedType(typeof(ComType), nameof(ComType))]
[JsonDerivedType(typeof(ObjectType), nameof(ObjectType))]
internal class BaseType(string name, string? namespaceName)
{
    public string Name { get; set; } = name;

    [JsonIgnore]
    public string? Namespace { get; set; } = namespaceName;

    public List<string>? Attributes { get; set; }

    public List<CustomAttribute>? CustomAttributes { get; set; }

    public string GetFullName() => Utils.GetFullTypeName(Namespace, Name);
}
