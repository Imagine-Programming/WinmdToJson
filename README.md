# WinmdToJson

## Overview
`WinmdToJson` is a tool developed for converting Windows Metadata (WinMD) files into JSON format. This utility is designed to facilitate the processing and analysis of WinMD files by converting them into a more accessible JSON format.

## Building
To build the `WinmdToJson` tool from its GitHub repository, you would typically follow these general steps, although specific instructions should ideally be provided in the repository:

1. **Clone the Repository**: Use Git to clone the repository to your local machine. 
   ```
   git clone https://github.com/ricknijhuis/WinmdToJson
   ```

2. **Open the Solution**: If the project includes a Visual Studio solution file (`.sln`), open it in Visual Studio.

3. **Restore NuGet Packages**: If the project relies on NuGet packages, restore them by right-clicking on the solution in Visual Studio and selecting "Restore NuGet Packages."

4. **Build the Solution**: Build the solution by right-clicking on the solution in Visual Studio and selecting "Build Solution," or by using the Build menu.

Since the repository is written entirely in C#, it's likely that Visual Studio is the intended development environment. However, without specific build instructions in the repository, these steps are based on general practices for C# projects and might need adjustments based on the project's specific setup.

## Using the Tool
The tool supports several command-line arguments to customize its operation:

- `--input`: Specifies the path to the WinMD file.
- `--output`: Defines the path to the output directory where the JSON files will be saved.
- `--namespace`: Specifies a specific namespace to load. If not provided, the tool loads everything.

### Example Usage
```
WinmdToJson --input path/to/winmdfile.winmd --output path/to/outputdir --namespace desiredNamespace
```

### Prerequisites
- Knowledge of C# (as the project is entirely in C#).
- Basic understanding of WinMD files and their structure.

### Installation and Usage
- Detailed instructions for installation and usage are currently not provided in the repository. (Consider adding a section here detailing how users can install and run your project.)

## About the Project
This tool is aimed at developers and users who need to interact with or analyze Windows Metadata in a more flexible format like JSON.

## Languages Used
- C# (100%)

## Contributing
Contributions to `WinmdToJson` are welcome. Please follow the standard open-source contribution guidelines when proposing changes or improvements.

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE.txt) file for more details.
