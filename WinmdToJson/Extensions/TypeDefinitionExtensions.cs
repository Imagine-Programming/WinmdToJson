using System.Reflection;
using System.Reflection.Metadata;
using Win32MetadataJsonGen.Decoders;
using Win32MetadataJsonGen.Types;

namespace Win32MetadataJsonGen.Extensions;
internal static class TypeDefinitionExtensions
{
    public static BaseType ToType(this TypeDefinition type,
        string? namespaceName,
        string typeName,
        MetadataReader reader,
        TypeDecoder decoder)
    {
        if (type.BaseType.IsNil)
            return new ComType(typeName, namespaceName);

        string? baseTypeName;

        if (type.BaseType.Kind == HandleKind.TypeReference)
        {
            var baseType = reader.GetTypeReference((TypeReferenceHandle)type.BaseType);
            baseTypeName = reader.GetString(baseType.Name);
        }
        else if (type.BaseType.Kind == HandleKind.TypeSpecification)
        {
            var baseType = reader.GetTypeSpecification((TypeSpecificationHandle)type.BaseType);
            var signature = baseType.DecodeSignature(decoder, null);
            baseTypeName = signature.Value!.Name;
        }
        else
        {
            var baseType = reader.GetTypeDefinition((TypeDefinitionHandle)type.BaseType);
            baseTypeName = reader.GetString(baseType.Name);
        }

        if (baseTypeName != null)
        {
            if (baseTypeName == "MulticastDelegate")
                return new FunctionPointerType(typeName, namespaceName);
            if (baseTypeName == "NativeType")
                return new PrimitiveType(typeName);
            if (baseTypeName == "ValueType")
            {
                if (type.Attributes.HasFlag(TypeAttributes.ExplicitLayout))
                    return new UnionType(typeName, namespaceName);
                return new StructType(typeName, namespaceName);
            }
            if (baseTypeName == "Enum")
                return new EnumType(typeName, namespaceName, new Reference<BaseType>());
            if (baseTypeName == "Object")
                return new ObjectType(typeName, namespaceName);
            if (baseTypeName == "Attribute")
                return new BaseType(typeName, namespaceName);
        }

        throw new NotImplementedException();
    }
}
