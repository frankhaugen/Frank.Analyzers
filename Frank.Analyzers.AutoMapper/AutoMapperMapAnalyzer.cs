using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Frank.Analyzers.AutoMapper;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AutoMapperMapAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DiagnosticDescriptors.AutoMapperMap);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;
        var expression = invocation.Expression;
        var simpleMemberAccessExpression = expression as MemberAccessExpressionSyntax;
        
        if (simpleMemberAccessExpression == null)
        {
            return;
        }

        if (simpleMemberAccessExpression == null || !simpleMemberAccessExpression.Name.Identifier.Text.Equals("Map"))
        {
            return;
        }

        // Check if there is exactly one type argument
        // if (simpleMemberAccessExpression.Length == 1)
        // {
        //     context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.AutoMapperMap, invocation.GetLocation()));
        // }
    }
}