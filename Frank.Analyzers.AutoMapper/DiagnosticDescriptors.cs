using Microsoft.CodeAnalysis;

namespace Frank.Analyzers.AutoMapper;


// | AUTO001 | Maintainability | Error    | .Map<>() should be used with two generic arguments. |

public static class DiagnosticDescriptors
{
    public static readonly DiagnosticDescriptor AutoMapperMap = new DiagnosticDescriptor(
        "AUTO001",
        "Maintainability",
        ".Map<>() should be used with two generic arguments",
        "AutoMapper .Map<>() should be used with two generic arguments.",
        DiagnosticSeverity.Error,
        true);
}