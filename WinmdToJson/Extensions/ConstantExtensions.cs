using System.Reflection.Metadata;

namespace Win32MetadataJsonGen.Extensions;
internal static class ConstantExtensions
{
    public static string GetValue(this Constant constant, MetadataReader reader)
    {
        if (!constant.Value.IsNil)
        {
            var blobReader = reader.GetBlobReader(constant.Value);

            return constant.TypeCode switch
            {
                ConstantTypeCode.Boolean => blobReader.ReadBoolean().ToString(),
                ConstantTypeCode.Char => blobReader.ReadChar().ToString(),
                ConstantTypeCode.SByte => blobReader.ReadSByte().ToString(),
                ConstantTypeCode.Byte => blobReader.ReadByte().ToString(),
                ConstantTypeCode.Int16 => blobReader.ReadInt16().ToString(),
                ConstantTypeCode.UInt16 => blobReader.ReadUInt16().ToString(),
                ConstantTypeCode.Int32 => blobReader.ReadInt32().ToString(),
                ConstantTypeCode.UInt32 => blobReader.ReadUInt32().ToString(),
                ConstantTypeCode.Int64 => blobReader.ReadInt64().ToString(),
                ConstantTypeCode.UInt64 => blobReader.ReadUInt64().ToString(),
                ConstantTypeCode.Single => blobReader.ReadSingle().ToString(),
                ConstantTypeCode.Double => blobReader.ReadDouble().ToString(),
                ConstantTypeCode.String => blobReader.ReadConstant(ConstantTypeCode.String)!.ToString()!,
                ConstantTypeCode.Invalid => throw new NotImplementedException(),
                ConstantTypeCode.NullReference => throw new NotImplementedException(),
                _ => throw new InvalidOperationException(),
            };
        }
        else if (constant.TypeCode == ConstantTypeCode.String)
            return "null";

        throw new InvalidOperationException();
    }
}
