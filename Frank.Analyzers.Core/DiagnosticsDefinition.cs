using Microsoft.CodeAnalysis;

namespace Frank.Analyzers.Core.DiagnosticsProviders;

public class DiagnosticsDefinition
{
    
}

public class DiagnosticsProvider
{
    public static readonly DiagnosticDescriptor AutoPropertyRule = new(
        id: "FRANK0010",
        title: "Auto property should be used",
        messageFormat: "Auto property should be used",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Auto property should be used."
    );
}