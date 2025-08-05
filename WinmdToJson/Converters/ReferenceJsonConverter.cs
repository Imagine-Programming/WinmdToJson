using System.Text.Json;
using System.Text.Json.Serialization;
using Win32MetadataJsonGen.Types;

namespace Win32MetadataJsonGen.Converters;

internal class ReferenceJsonConverter : JsonConverter<Reference<BaseType>>
{
    public override Reference<BaseType> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException("Deserialization not supported");
    }

    public override void Write(Utf8JsonWriter writer, Reference<BaseType> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        
        if (value.Kind is not null)
            writer.WriteString("Kind", value.Kind);
        if (value.Namespace is not null)
            writer.WriteString("Namespace", value.Namespace);
        if (value.Type is not null)
            writer.WriteString("Type", value.Type);
        
        if (value.Value is ArrayType arrayType)
        {
            writer.WriteNumber("Rank", arrayType.Rank);
            
            if (arrayType.Sizes != null)
            {
                writer.WriteStartArray("Sizes");
                foreach (var size in arrayType.Sizes)
                {
                    writer.WriteNumberValue(size);
                }
                writer.WriteEndArray();
            }
            
            if (arrayType.LowerBounds != null)
            {
                writer.WriteStartArray("LowerBounds");
                foreach (var bound in arrayType.LowerBounds)
                {
                    writer.WriteNumberValue(bound);
                }
                writer.WriteEndArray();
            }
            
            if (arrayType.ElementType != null)
            {
                writer.WritePropertyName("ElementType");
                JsonSerializer.Serialize(writer, arrayType.ElementType, options);
            }
        }
        
        writer.WriteEndObject();
    }
}