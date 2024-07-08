using Microsoft.CodeAnalysis;

namespace Frank.Analyzers.Core;

public interface IDiagnosticDescriptorProvider
{
    DiagnosticDescriptor GetDescriptor();
}