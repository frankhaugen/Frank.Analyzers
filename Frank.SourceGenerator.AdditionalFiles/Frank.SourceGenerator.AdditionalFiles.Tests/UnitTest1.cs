using Frank.SourceGenerator.AdditionalFiles.Tests.Playground;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Xunit.Abstractions;

namespace Frank.SourceGenerator.AdditionalFiles.Tests;

public class EmbeddedResourceGeneratorTests
{
    private readonly ITestOutputHelper _outputHelper;

    public EmbeddedResourceGeneratorTests(ITestOutputHelper outputHelper)
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

file class AnalyzerAdditionalText : AdditionalText
{
    private readonly SourceText _content;

    internal AnalyzerAdditionalText(string path, SourceText content)
    {
        Path = path;
        _content = content;
    }

    public override string Path { get; }

    public override SourceText GetText(CancellationToken cancellationToken = default) => _content;
}

file class TestOptionsProvider : AnalyzerConfigOptionsProvider
{
    private readonly TestAnalyzerConfigOptions _options = new();

    public TestOptionsProvider()
    {
        _options.Add("build_property.rootnamespace", "Frank.GameEngine.Assets");
        _options.Add("build_property.projectdir", @"C:\repos\frankhaugen\Frank.GameEngine\src\Frank.GameEngine.Assets");
    }

    public override AnalyzerConfigOptions GlobalOptions => _options;

    public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => _options;

    public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => _options;
}

file class TestAnalyzerConfigOptions : AnalyzerConfigOptions
{
    private readonly Dictionary<string, string> _options = new();

    public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
    {
        if (_options.TryGetValue(key, out var result))
        {
            value = result;
            return true;
        }

        value = null;
        return false;
    }

    public void Add(string key, string value)
    {
        _options.Add(key, value);
    }
}