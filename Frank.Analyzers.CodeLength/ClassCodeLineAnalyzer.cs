using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using Frank.Analyzers.Core;
using Frank.Analyzers.Core.DiagnosticsProviders;

namespace Frank.Analyzers.CodeLength;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ClassCodeLineAnalyzer : DiagnosticAnalyzer
{
	private DiagnosticDescriptor Rule => new TooManyLinesInClassDescriptorProvider().GetDescriptor();
		
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];
		
	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.ReportDiagnostics);
		context.EnableConcurrentExecution();
		context.RegisterSymbolAction(TypeSymbolAction, SymbolKind.NamedType);
	}

	private void TypeSymbolAction(SymbolAnalysisContext obj)
	{
		if (obj.Symbol is not INamedTypeSymbol typeSymbol)
			return;

		if (typeSymbol.DeclaringSyntaxReferences.Length == 0)
			return;

		var syntaxTree = typeSymbol.DeclaringSyntaxReferences[0].SyntaxTree;

		if (syntaxTree.TryGetText(out var resultText) && resultText.Lines.Count > CodeLengthSettings.ClassMaxLines)
		{
			obj.ReportDiagnostic(new DiagnosticBuilder().WithDescriptor(Rule).WithLocation(typeSymbol.Locations[0]).WithArguments(typeSymbol.Name, CodeLengthSettings.ClassMaxLines).Build());
		}
	}
}