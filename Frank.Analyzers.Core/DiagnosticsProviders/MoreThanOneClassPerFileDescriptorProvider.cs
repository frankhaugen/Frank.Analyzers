using Microsoft.CodeAnalysis;

namespace Frank.Analyzers.Core.DiagnosticsProviders;

public class MoreThanOneClassPerFileDescriptorProvider : IDiagnosticDescriptorProvider
{
    public DiagnosticDescriptor GetDescriptor() =>
        new DiagnosticDescriptorBuilder()
            .WithIdBuilder(new DiagnosticIdBuilder().WithCategory(DiagnosticCategories.Maintainability).WithId(12))
            .WithTitle("More than one class per file")
            .WithMessageFormat("File '{0}' contains more than one class.")
            .WithCategory(DiagnosticCategories.Maintainability)
            .WithDefaultSeverity(DiagnosticSeverity.Warning)
            .WithIsEnabledByDefault(true)
            .WithDescription("Files should contain only one class.")
            .Build();
}