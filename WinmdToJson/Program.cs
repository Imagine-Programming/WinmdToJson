using System.CommandLine;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using Win32MetadataJsonGen;

var inputOption = new Option<FileInfo>(
        name: "--input",
        description: "Path to winmd file"
    );

var outputOption = new Option<DirectoryInfo>(
        name: "--output",
        description: "directory where output files are created",
        getDefaultValue: () => new DirectoryInfo("win32")
    );

var namespaceOption = new Option<List<string>>(
        name: "--namespaces",
        description: "namespaces to load, default is all",
        getDefaultValue: () => new()
    );

var rootCommand = new RootCommand("Json generator of the Win32 API");
rootCommand.AddOption(inputOption);
rootCommand.AddOption(outputOption);
rootCommand.AddOption(namespaceOption);
rootCommand.SetHandler(async (input, output, namespaces) =>
{

    var stopwatch = new Stopwatch();
    stopwatch.Start();

    using var metadataFileStream = input.OpenRead();
    using var peReader = new PEReader(metadataFileStream);

    var reader = new Reader(peReader.GetMetadataReader());
    var result = reader.Read(namespaces);

    var writer = new Writer();
    await writer.Write(output, result);

    stopwatch.Stop();

    Console.WriteLine("Json generated in: {0} ms", stopwatch.ElapsedMilliseconds);

}, inputOption, outputOption, namespaceOption);

return rootCommand.InvokeAsync(args).Result;