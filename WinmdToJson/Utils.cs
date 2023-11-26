namespace Win32MetadataJsonGen;

public static class Utils
{
    public static string GetFullTypeName(string? namespaceName, string typeName)
    {
        if (string.IsNullOrEmpty(namespaceName))
            return typeName;
        else return namespaceName + "." + typeName;
    }

    public static Guid ParseGuid(List<uint> numbers)
    {
        if (numbers == null || numbers.Count != 11)
            throw new ArgumentException("Invalid number of elements for a GUID.");

        int a = (int)numbers[0];    // First number as int
        short b = (short)numbers[1]; // Second number as short
        short c = (short)numbers[2]; // Third number as short

        // Remaining numbers as bytes
        byte d = (byte)numbers[3];
        byte e = (byte)numbers[4];
        byte f = (byte)numbers[5];
        byte g = (byte)numbers[6];
        byte h = (byte)numbers[7];
        byte i = (byte)numbers[8];
        byte j = (byte)numbers[9];
        byte k = (byte)numbers[10];

        return new Guid(a, b, c, d, e, f, g, h, i, j, k);
    }
}
