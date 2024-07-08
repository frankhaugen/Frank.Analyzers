using Microsoft.CodeAnalysis;

namespace Frank.Analyzers.Core.DiagnosticsProviders;

public class TooManyLinesInMethodDescriptorProvider : IDiagnosticDescriptorProvider
{
    public DiagnosticDescriptor GetDescriptor() =>
        new DiagnosticDescriptorBuilder()
            .WithIdBuilder(new DiagnosticIdBuilder().WithCategory(DiagnosticCategories.Maintainability).WithId(10))
            .WithTitle("Too many lines in method")
            .WithMessageFormat("Method '{0}' has too many lines ({1}).")
            .WithCategory(DiagnosticCategories.Maintainability)
            .WithDefaultSeverity(DiagnosticSeverity.Warning)
            .WithIsEnabledByDefault(true)
            .WithDescription("Methods should not have too many lines.")
            .Build();
}