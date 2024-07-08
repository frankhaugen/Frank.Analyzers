using Microsoft.CodeAnalysis;

namespace Frank.Refactoring.AutoProperties.Internals;

internal interface IDiagnosticDescriptorProvider
{
    DiagnosticDescriptor GetDescriptor();
}