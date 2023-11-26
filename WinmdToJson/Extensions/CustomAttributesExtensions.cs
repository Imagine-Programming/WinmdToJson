using System.Reflection;
using System.Reflection.Metadata;
using Win32MetadataJsonGen.Decoders;
using CustomAttribute = Win32MetadataJsonGen.Types.CustomAttribute;

namespace Win32MetadataJsonGen.Extensions;
internal static class CustomAttributesExtensions
{
    public static string GetAttributeTypeName(this System.Reflection.Metadata.CustomAttribute attribute, MetadataReader reader)
    {
        switch (attribute.Constructor.Kind)
        {
            case HandleKind.MethodDefinition:
                {
                    var method = reader.GetMethodDefinition((MethodDefinitionHandle)attribute.Constructor);
                    var type = reader.GetTypeDefinition(method.GetDeclaringType());
                    return reader.GetString(type.Namespace) + reader.GetString(type.Name);
                }
            case HandleKind.MemberReference:
                {
                    var member = reader.GetMemberReference((MemberReferenceHandle)attribute.Constructor);
                    var type = reader.GetTypeReference((TypeReferenceHandle)member.Parent);
                    return Utils.GetFullTypeName(reader.GetString(type.Namespace), reader.GetString(type.Name));
                }
            default:
                throw new InvalidOperationException($"Attribute type not supported: {attribute.Constructor.Kind}");
        }
    }

    public static List<CustomAttribute>? ToAttributeType(this CustomAttributeHandleCollection customAttributes,
        AttributeDecoder decoder,
        MetadataReader reader)
    {
        var attributes = customAttributes
            .Select(reader.GetCustomAttribute)
            .Select(c =>
            {
                var decoded = c.DecodeValue(decoder);
                var fixedParams = decoded.FixedArguments.Select(k => k.Value).ToList();
                var namedParams = decoded.NamedArguments.Select(k => new KeyValuePair<string, object?>(k.Name!, k.Value)).ToList();

                if (fixedParams.Count == 0)
                    fixedParams = null;

                if (namedParams.Count == 0)
                    namedParams = null;

                return new CustomAttribute(c.GetAttributeTypeName(reader))
                {
                    NamedParameters = namedParams,
                    FixedParameters = fixedParams
                };
            })
            .ToList();

        if (attributes.Count > 0)
            return attributes;

        return null;
    }
}
