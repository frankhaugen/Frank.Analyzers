using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Frank.SourceGenerator.AdditionalFiles.Tests;

internal class AnalyzerAdditionalText : AdditionalText
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