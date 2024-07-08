using Microsoft.CodeAnalysis;

namespace Frank.Analyzers.CodeLength.Internals;

internal interface IDiagnosticDescriptorProvider
{
    DiagnosticDescriptor GetDescriptor();
}