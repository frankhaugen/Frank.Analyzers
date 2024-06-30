using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace Frank.Analyzers.CodeLength
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class FrankAnalyzersCodeLengthAnalyzer : DiagnosticAnalyzer
	{
		public const int MaxCodeLine = 50;
		public const string DiagnosticId = "FA1000";
		public const string Category = "CodeQuality";
		public const string Title = "Code length is too long";
		public const string MessageFormat = "Code-file '{0}' contains more than {1} lines";
		public const DiagnosticSeverity Severity = DiagnosticSeverity.Error;

		public static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title, MessageFormat, Category, Severity, true, "Checks the number of lines of code");

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);
		public Diagnostic CreateDiagnostic(string filePath) => Diagnostic.Create(Rule, Location.None, Path.GetFileNameWithoutExtension(filePath), MaxCodeLine);

		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze);
			context.EnableConcurrentExecution();
			context.RegisterSyntaxTreeAction(SyntaxTreeAction);

			context.RegisterCompilationAction(CompilationAction);

			context.RegisterSemanticModelAction(SemanticModelAction);
		}

		private void SyntaxTreeAction(SyntaxTreeAnalysisContext obj)
		{
			var isInvalid = obj.Tree.TryGetText(out var resultText) && resultText.Lines.Count(line => line.Text.ToString().Contains($";")) > MaxCodeLine;
			if (isInvalid) obj.ReportDiagnostic(CreateDiagnostic(obj.Tree.FilePath));
		}

		private void SemanticModelAction(SemanticModelAnalysisContext obj)
		{
			var illegalTrees = GetIllegalSyntaxTrees(obj.SemanticModel.Compilation);

			foreach (var illegalTree in illegalTrees)
			{
				obj.ReportDiagnostic(CreateDiagnostic(illegalTree.FilePath));
			}
		}

		private void CompilationAction(CompilationAnalysisContext obj)
		{
			var illegalTrees = GetIllegalSyntaxTrees(obj.Compilation);

			foreach (var illegalTree in illegalTrees)
			{
				obj.ReportDiagnostic(Diagnostic.Create(Rule, Location.None, Path.GetFileNameWithoutExtension(illegalTree.FilePath), MaxCodeLine));
			}
		}

		private static List<SyntaxTree> GetIllegalSyntaxTrees(Compilation compilation) => compilation.SyntaxTrees.AsParallel().Where(x => x.TryGetText(out var resultText) && resultText.Lines.Count(line => line.Text.ToString().Contains($";")) > MaxCodeLine).ToList();
	}
}