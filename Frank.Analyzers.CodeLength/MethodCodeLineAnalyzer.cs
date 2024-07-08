using System.Collections.Immutable;
using Frank.Analyzers.CodeLength.Internals;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Frank.Analyzers.CodeLength;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class MethodCodeLineAnalyzer : DiagnosticAnalyzer
{
    private DiagnosticDescriptor Rule => new TooManyLinesInMethodDescriptorProvider().GetDescriptor();
		
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];
		
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(MethodSymbolAction, SymbolKind.Method);
    }

    private void MethodSymbolAction(SymbolAnalysisContext obj)
    {
        if (obj.Symbol is not IMethodSymbol methodSymbol)
            return;

        if (methodSymbol.DeclaringSyntaxReferences.Length == 0)
            return;

        var syntaxTree = methodSymbol.DeclaringSyntaxReferences[0].SyntaxTree;

        if (syntaxTree.TryGetText(out var resultText) && resultText.Lines.Count(line => line.Text!.ToString().Contains($";")) > CodeLengthSettings.MethodMaxLines)
        {
            obj.ReportDiagnostic(new DiagnosticBuilder().WithDescriptor(Rule).WithLocation(methodSymbol.Locations[0]).WithArguments(methodSymbol.Name, CodeLengthSettings.MethodMaxLines).Build());
        }
    }
}