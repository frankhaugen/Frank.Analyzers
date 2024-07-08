using Microsoft.CodeAnalysis;

namespace Frank.Analyzers.Core.DiagnosticsProviders;

public class PropertyShouldNotHaveBackingFieldDescriptorProvider : IDiagnosticDescriptorProvider
{
    public DiagnosticDescriptor GetDescriptor() =>
        new DiagnosticDescriptorBuilder()
            .WithIdBuilder(new DiagnosticIdBuilder().WithCategory(DiagnosticCategories.Design).WithId(13))
            .WithTitle("Property should not have backing field")
            .WithMessageFormat("Property '{0}' should not have a backing field.")
            .WithCategory(DiagnosticCategories.Design)
            .WithDefaultSeverity(DiagnosticSeverity.Error)
            .WithIsEnabledByDefault(true)
            .WithDescription("Properties should not have backing fields.")
            .Build();
}