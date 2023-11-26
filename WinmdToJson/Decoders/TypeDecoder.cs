using System.Collections.Immutable;
using System.Reflection.Metadata;
using Win32MetadataJsonGen.Extensions;
using Win32MetadataJsonGen.Types;

namespace Win32MetadataJsonGen.Decoders;

internal class TypeDecoder(Dictionary<string, Reference<BaseType>> registry)
    : ISignatureTypeProvider<Reference<BaseType>, int?>
{
    private readonly Dictionary<string, Reference<BaseType>> _registry = registry;

    public Reference<BaseType> GetArrayType(Reference<BaseType> elementType, ArrayShape shape)
    {
        ArgumentNullException.ThrowIfNull(elementType.Value, nameof(elementType));

        // we don't store array types in the registry, we simply create a new one to be used directly
        var reference = new Reference<BaseType>
        {
            Value = new ArrayType(elementType.Value.Name, elementType.Value.Namespace, shape)
        };

        return reference;
    }

    public Reference<BaseType> GetByReferenceType(Reference<BaseType> elementType)
    {
        throw new NotImplementedException();
    }

    public Reference<BaseType> GetFunctionPointerType(MethodSignature<Reference<BaseType>> signature)
    {
        throw new NotImplementedException();
    }

    public Reference<BaseType> GetGenericInstantiation(Reference<BaseType> genericType, ImmutableArray<Reference<BaseType>> typeArguments)
    {
        throw new NotImplementedException();
    }

    public Reference<BaseType> GetGenericMethodParameter(int? genericContext, int index)
    {
        throw new NotImplementedException();
    }

    public Reference<BaseType> GetGenericTypeParameter(int? genericContext, int index)
    {
        throw new NotImplementedException();
    }

    public Reference<BaseType> GetModifiedType(Reference<BaseType> modifier, Reference<BaseType> unmodifiedType, bool isRequired)
    {
        throw new NotImplementedException();
    }

    public Reference<BaseType> GetPinnedType(Reference<BaseType> elementType)
    {
        throw new NotImplementedException();
    }

    public Reference<BaseType> GetPointerType(Reference<BaseType> elementType)
    {
        ArgumentNullException.ThrowIfNull(elementType.Value, nameof(elementType));

        // we don't store pointer types in the registry, we simply create a new one to be used directly
        var reference = new Reference<BaseType>
        {
            Value = new PointerType(elementType.Value.Name, elementType.Value.Namespace)
        };

        return reference;
    }

    public Reference<BaseType> GetPrimitiveType(PrimitiveTypeCode typeCode)
    {
        // should always be available as these are created at startup
        return _registry[typeCode.ToString()];
    }

    public Reference<BaseType> GetSZArrayType(Reference<BaseType> elementType)
    {
        throw new NotImplementedException();
    }

    public Reference<BaseType> GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
    {
        var type = reader.GetTypeDefinition(handle);
        var namespaceName = reader.GetString(type.Namespace);
        var typeName = reader.GetString(type.Name);
        var fullTypeName = Utils.GetFullTypeName(namespaceName, typeName);

        var reference = _registry.GetOrCreate(fullTypeName);

        reference.Value ??= new BaseType(typeName, namespaceName);

        return reference;
    }

    public Reference<BaseType> GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
    {
        var type = reader.GetTypeReference(handle);
        var namespaceName = reader.GetString(type.Namespace);
        var typeName = reader.GetString(type.Name);
        var fullTypeName = Utils.GetFullTypeName(namespaceName, typeName);

        var reference = _registry.GetOrCreate(fullTypeName);

        reference.Value ??= new BaseType(typeName, namespaceName);

        return reference;
    }

    public Reference<BaseType> GetTypeFromSpecification(MetadataReader reader, int? genericContext, TypeSpecificationHandle handle, byte rawTypeKind)
    {
        throw new NotImplementedException();
    }
}
