using Microsoft.CodeAnalysis;

namespace Frank.Analyzers.Core.DiagnosticsProviders;

public class FailedToGenerateAdditionalFilesResourceDescriptorProvider : IDiagnosticDescriptorProvider
{
    public DiagnosticDescriptor GetDescriptor() =>
        new DiagnosticDescriptorBuilder()
            .WithIdBuilder(new DiagnosticIdBuilder().WithCategory(DiagnosticCategories.Usage).WithId(13))
            .WithTitle("Failed to generate additional files resource")
            .WithMessageFormat("Failed to generate additional files resource.")
            .WithCategory(DiagnosticCategories.Usage)
            .WithDefaultSeverity(DiagnosticSeverity.Error)
            .WithIsEnabledByDefault(true)
            .WithDescription("Failed to generate additional files resource.")
            .Build();
}