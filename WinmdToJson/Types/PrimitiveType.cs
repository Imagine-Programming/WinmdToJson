using System.Reflection.Metadata;
using Win32MetadataJsonGen.Extensions;

namespace Win32MetadataJsonGen.Types;

internal class PrimitiveType(string name)
    : BaseType(name, null)
{
    public PrimitiveType(PrimitiveTypeCode code)
        : this(code.ToString()) { }

    public PrimitiveType(ConstantTypeCode code)
        : this(code.ToPrimitiveTypeCode().ToString()) { }
}
