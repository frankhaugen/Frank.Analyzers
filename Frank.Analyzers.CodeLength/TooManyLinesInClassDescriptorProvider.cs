using Frank.Analyzers.CodeLength.Internals;
using Microsoft.CodeAnalysis;

namespace Frank.Analyzers.CodeLength;

public class TooManyLinesInClassDescriptorProvider : IDiagnosticDescriptorProvider
{
    public DiagnosticDescriptor GetDescriptor() =>
        new DiagnosticDescriptorBuilder()
            .WithIdBuilder(new DiagnosticIdBuilder().WithCategory(DiagnosticCategories.Maintainability).WithId(11))
            .WithTitle("Too many lines in class")
            .WithMessageFormat("Class '{0}' has too many lines ({1}).")
            .WithCategory(DiagnosticCategories.Maintainability)
            .WithDefaultSeverity(DiagnosticSeverity.Warning)
            .WithIsEnabledByDefault(true)
            .WithDescription("Classes should not have too many lines.")
            .Build();
}