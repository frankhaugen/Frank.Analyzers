using Microsoft.CodeAnalysis;

namespace Frank.SourceGenerators.CppInteropts.Internals;

internal interface IDiagnosticDescriptorProvider
{
    DiagnosticDescriptor GetDescriptor();
}