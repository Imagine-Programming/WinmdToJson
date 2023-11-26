using System.Reflection.Metadata;

namespace Win32MetadataJsonGen.Extensions;

internal static class ConstantTypeCodeExtensions
{
    public static PrimitiveTypeCode ToPrimitiveTypeCode(this ConstantTypeCode code) => code switch
    {
        ConstantTypeCode.Boolean => PrimitiveTypeCode.Boolean,
        ConstantTypeCode.Char => PrimitiveTypeCode.Char,
        ConstantTypeCode.SByte => PrimitiveTypeCode.SByte,
        ConstantTypeCode.Byte => PrimitiveTypeCode.Byte,
        ConstantTypeCode.Int16 => PrimitiveTypeCode.Int16,
        ConstantTypeCode.UInt16 => PrimitiveTypeCode.UInt16,
        ConstantTypeCode.Int32 => PrimitiveTypeCode.Int32,
        ConstantTypeCode.UInt32 => PrimitiveTypeCode.UInt32,
        ConstantTypeCode.Int64 => PrimitiveTypeCode.Int64,
        ConstantTypeCode.UInt64 => PrimitiveTypeCode.UInt64,
        ConstantTypeCode.Single => PrimitiveTypeCode.Single,
        ConstantTypeCode.Double => PrimitiveTypeCode.Double,
        ConstantTypeCode.String => PrimitiveTypeCode.String,
        _ => throw new NotImplementedException(),
    };
}
