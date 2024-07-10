# Cpp Interopts Source Generator

This source generator generates C++ interop code for a given C# assembly.

## Usage

Add the following to your project file:

```xml
<ItemGroup>
  <PackageReference Include="Frank.Libraries.CppInteropts" Version="0.1.0" />
</ItemGroup>
```

Then, add the following to your project file:

```xml
<ItemGroup>
  <AdditionalFiles Include="Assets/**" />
</ItemGroup>
```

This will generate C++ interop code for all C++ files in the `Assets` directory as C# files.