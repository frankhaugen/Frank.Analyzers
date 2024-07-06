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

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [AutoPropertyRule];

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.PropertyDeclaration);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;
            var propertyDeclarationWalker = new PropertyDeclarationWalker(context.SemanticModel);
            
            propertyDeclarationWalker.Visit(propertyDeclaration);
            
            var diagnostics = propertyDeclarationWalker.PropertiesWithBackingFields.Select(propertyWithBackingField => Diagnostic.Create(AutoPropertyRule, propertyWithBackingField.Property.GetLocation(), propertyWithBackingField.Property.Identifier.Text)).ToList();
            diagnostics.ForEach(context.ReportDiagnostic);
        }
    }
}