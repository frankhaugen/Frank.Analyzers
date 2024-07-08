using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Frank.Analyzers.Tests.TestingInfrastructure;

internal class TestAnalyzerConfigOptions : AnalyzerConfigOptions
{
    private readonly Dictionary<string, string> _options = new();

    public TestAnalyzerConfigOptions(params (string key, string value)[] options)
    {
        foreach (var (key, value) in options)
        {
            _options.Add(key, value);
        }
    }

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