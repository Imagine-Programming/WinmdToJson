using System.Reflection.Metadata;
using Win32MetadataJsonGen.Extensions;
using Win32MetadataJsonGen.Types;

namespace Win32MetadataJsonGen.Decoders;

internal class AttributeDecoder(Dictionary<string, Reference<BaseType>> registry) : ICustomAttributeTypeProvider<Reference<BaseType>>
{
    private readonly Dictionary<string, Reference<BaseType>> _registry = registry;
    public Reference<BaseType> GetPrimitiveType(PrimitiveTypeCode typeCode) => _registry[typeCode.ToString()];

    public Reference<BaseType> GetSystemType() => _registry[PrimitiveTypeCode.Object.ToString()];

    public Reference<BaseType> GetSZArrayType(Reference<BaseType> elementType)
        => throw new NotImplementedException();

    public Reference<BaseType> GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
        => throw new NotImplementedException();

    public Reference<BaseType> GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
    {
        var type = reader.GetTypeReference(handle);
        var typeName = Utils.GetFullTypeName(reader.GetString(type.Namespace), reader.GetString(type.Name));

        return _registry.GetOrCreate(typeName!);
    }

    public Reference<BaseType> GetTypeFromSerializedName(string name)
        => throw new NotImplementedException();

    public PrimitiveTypeCode GetUnderlyingEnumType(Reference<BaseType> type)
    {
        if (type.Value is EnumType enumType && enumType.Type.Value != null)
            return (PrimitiveTypeCode)Enum.Parse(typeof(PrimitiveTypeCode), enumType.Type!.Value!.Name);
        return PrimitiveTypeCode.Int32;
    }

    public bool IsSystemType(Reference<BaseType> type) => type.Value is PrimitiveType;
}
