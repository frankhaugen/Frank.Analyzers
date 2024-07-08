using Microsoft.CodeAnalysis;

namespace Frank.SourceGenerator.AdditionalFiles.Internals;

internal interface IDiagnosticDescriptorProvider
{
    DiagnosticDescriptor GetDescriptor();
}