using Microsoft.CodeAnalysis;

namespace Frank.Refactoring.AutoProperties;

internal static class DiagnosticProvider
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