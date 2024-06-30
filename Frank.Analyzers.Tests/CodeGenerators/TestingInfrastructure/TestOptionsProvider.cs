using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Frank.SourceGenerator.AdditionalFiles.Tests;

internal class TestOptionsProvider : AnalyzerConfigOptionsProvider
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