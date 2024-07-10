# Additional Files

This is an analyzer that will generate a and a class named `AdditionalFilesHelper` that will contain a list of all 
additional files in the project, structured as nested classes.

The generated class will be placed in the namespace of the referencing project.

## Usage

To use this analyzer, add the following to your project file:

```xml
<ItemGroup>
  <AdditionalFiles Include="Assets\**" />
</ItemGroup>
```

This will include all files in the `Assets` directory and its subdirectories.

## Example

Given the following project structure:

```
Project
│   Project.csproj
│
└───Assets
    │   file1.txt
    │   file2.txt
    │
    └───Subfolder
        │   file3.txt
        │   file4.txt
```

The generated class will look like this:

```csharp
namespace Project
{
    public static class AdditionalFilesHelper
    {
        public static class Assets
        {
            public static class Subfolder
            {
                public const string File3Txt = "Assets/Subfolder/file3.txt";
                public const string File4Txt = "Assets/Subfolder/file4.txt";
            }

            public const string File1Txt = "Assets/file1.txt";
            public const string File2Txt = "Assets/file2.txt";
        }
    }
}
```