﻿using Microsoft.CodeAnalysis.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Frank.SourceGenerator.AdditionalFiles.Tests;

internal class TestAnalyzerConfigOptions : AnalyzerConfigOptions
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