using Frank.SourceGenerator.AdditionalFiles.Tests.Playground;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.Reflection;
using Xunit.Abstractions;

namespace Frank.SourceGenerator.AdditionalFiles.Tests;

public class AdditionalFilesHelperGeneratorTests
{
    private readonly ITestOutputHelper _outputHelper;

    public AdditionalFilesHelperGeneratorTests(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    [Fact]
    public void Use()
    {
        var thing = AdditionalResources.JsonFiles.Pokemon;
        _outputHelper.WriteLine(thing.Length.ToString());
    }

    [Fact]
    public void Generate()
    {
        var inputCompilation = CSharpCompilation.Create("compilation",
            new[] { CSharpSyntaxTree.ParseText("public class R { }") },
            new[] { MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location) });

        var generator = new AdditionalFilesHelperGenerator();

        var generators = new ISourceGenerator[] { generator };
        var additionalFiles = new[]
        {
            new AnalyzerAdditionalText(@".\Assets\Teapot.obj", SourceText.From("Hello world")),
            new AnalyzerAdditionalText(@".\Assets\Models\Teapot.obj", SourceText.From("Hello world")),
            new AnalyzerAdditionalText(@".\Assets\Models\My Hole\State.obj", SourceText.From("Hello world")),
            new AnalyzerAdditionalText(@".\Assets\Models\Bob\MtL\Teapot.mtl", SourceText.From("Hello world")),
        };

        CSharpGeneratorDriver.Create(generators, optionsProvider: new TestOptionsProvider(), additionalTexts: additionalFiles).RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);

        _outputHelper.WriteLine(string.Join(Environment.NewLine, diagnostics.Select(x => x.GetMessage())));
        var syntaxTree = outputCompilation.SyntaxTrees.ElementAt(1);
        var text = syntaxTree.ToString();

        _outputHelper.WriteLine(text);
    }
}