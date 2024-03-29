# Frank.SourceGenerator.AdditionalFiles

The `Frank.SourceGenerator.AdditionalFiles` NuGet package provides a simple way to create a helper class for managing additional resources in your .NET projects. This assembly generates a robust and flexible helper class, tailored to your additional files, using Roslyn source generators.

## Features

- Supports all additional files you add to your .NET project.
- Generates a class member for each file, giving you easy access to your additional resources.
- Framework-agnostic: works with all project types that support source generators, including .NET Core, .NET Standard and .NET 5.
- Each additional file is turned into an embedded resource property, providing the file's content as a byte array.
- Nested classes are generated corresponding to the directories of the project structure, providing a familiar filing system for developers.
- Compatible with JetBrains Rider 2023.2 EAP 8 and higher.

## Usage

1. Add the `Frank.SourceGenerator.AdditionalFiles` NuGet package to your project.

2. Add additional files to your project - these files will be automatically processed by the source generator.

3. In your code, you can access your additional files via the `AdditionalResources` class, like this:

```C#
byte[] myFileContent = AdditionalResources.MyFileName;
```

This returns the content of the additional file `MyFileName` as a byte array, ready to be used in your application.

**Please note**: `MyFileName` should be the name of your additional file turned to a valid C# identifier according to certain transformation rules applied by the generator.

## Known Limitations

- File names are turned into a valid C# identifiers (method names) using the following transformation rules: non-alphanumeric characters are replaced with underscores, the first character of the resulting string is capitalized.

## Contribution

Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

## License

The `Frank.SourceGenerator.AdditionalFiles` NuGet package is provided under the MIT License. Please see the [LICENSE](https://github.com/frankhaugen/Frank.Analyzers/blob/main/LICENSE) file for more information.

