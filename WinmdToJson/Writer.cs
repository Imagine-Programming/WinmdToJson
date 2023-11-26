using System.Text.Json;
using System.Text.Json.Serialization;
using Win32MetadataJsonGen.Models;

namespace Win32MetadataJsonGen;

internal class Writer
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        Converters = {
            new JsonStringEnumConverter()
        }
    };

    public async Task Write(DirectoryInfo outputDir, ReadResult result)
    {
        outputDir.Create();

        var tasks = result.Namespaces
            .Where(n => n.Types.Count > 0)
            .Select(n => Write(outputDir, n))
            .ToList();

        await Task.WhenAll(tasks);
    }

    private async Task Write(DirectoryInfo outputDir, Namespace @namespace)
    {
        var filePath = @namespace.Name + ".json";
        var fullpath = Path.Combine(outputDir.FullName, filePath);

        Console.WriteLine("Creating: {0}", fullpath);

        using var stream = File.Open(fullpath, FileMode.Create, FileAccess.Write, FileShare.None);
        await JsonSerializer.SerializeAsync(stream, @namespace, _jsonOptions);
    }
}
