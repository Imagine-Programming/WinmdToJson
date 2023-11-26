using System.Reflection.Metadata;

namespace Win32MetadataJsonGen.Extensions;

internal static class NamespaceDefinitionExtensions
{
    public static string GetFullName(this NamespaceDefinition definition, MetadataReader reader)
    {
        string? name = null;

        NamespaceDefinition? child = definition;

        do
        {
            if (string.IsNullOrEmpty(name))
            {
                name = reader.GetString(child.Value.Name);
            }
            else
            {
                name = Utils.GetFullTypeName(reader.GetString(child.Value.Name), name);
            }

            if (!child.Value.Parent.IsNil)
            {
                child = reader.GetNamespaceDefinition(child.Value.Parent);
            }
            else
            {
                child = null;
            }

        } while (child != null);

        return name;
    }
}
