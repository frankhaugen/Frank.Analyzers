using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Frank.Analyzers.AutoProperties
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AutoPropertySyntaxAnalyzer : DiagnosticAnalyzer
    {
        public static readonly DiagnosticDescriptor AutoPropertyRule = new(
            id: "FR001",
            title: "Use auto property",
            messageFormat: "Property '{0}' can be converted to auto-property",
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(AutoPropertyRule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.PropertyDeclaration);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;

            if (propertyDeclaration.AccessorList?.Accessors.Count == 2 &&
                propertyDeclaration.AccessorList.Accessors.All(a => a.Body != null))
            {
                var diagnostic = Diagnostic.Create(AutoPropertyRule, propertyDeclaration.GetLocation(), propertyDeclaration.Identifier.Text);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}