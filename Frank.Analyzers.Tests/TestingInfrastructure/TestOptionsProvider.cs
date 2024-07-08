using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Frank.Analyzers.Tests.TestingInfrastructure;

internal class TestOptionsProvider : AnalyzerConfigOptionsProvider
{
    public override AnalyzerConfigOptions GlobalOptions => new TestAnalyzerConfigOptions(
    
        new( "build_property.rootnamespace", $"{GetType().Assembly.GetName().Name}" ),
        new( "build_property.projectdir", $"{new FileInfo(GetType().Assembly.Location).Directory?.FullName}" )
    );

    public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => GlobalOptions;

    public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => GlobalOptions;
}