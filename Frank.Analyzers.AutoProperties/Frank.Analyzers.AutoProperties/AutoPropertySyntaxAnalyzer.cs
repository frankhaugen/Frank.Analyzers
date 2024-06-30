using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Frank.Analyzers.AutoProperties;

/// <summary>
/// A sample analyzer that reports the company name being used in class declarations.
/// Traverses through the Syntax Tree and checks the name (identifier) of each class node.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AutoPropertySyntaxAnalyzer : DiagnosticAnalyzer
{
    private readonly Diagnostic Diagnostic = Diagnostic.Create(DiagnosticProvider.AutoPropertyRule, Location.None);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        
        void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is PropertyDeclarationSyntax propertyDeclarationSyntax)
            {
                if (propertyDeclarationSyntax.AccessorList is null)
                {
                    context.ReportDiagnostic(Diagnostic);
                }
            }
        }

        context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.PropertyDeclaration);
    }
    

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [DiagnosticProvider.AutoPropertyRule];
}