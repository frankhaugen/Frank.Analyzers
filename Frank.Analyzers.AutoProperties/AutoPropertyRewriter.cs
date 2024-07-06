using Frank.Analyzers.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace Frank.Analyzers.AutoProperties;


public class AutoPropertyRewriter : CSharpSyntaxRewriter
{
    private readonly SemanticModel _semanticModel;
    private readonly IEnumerable<PropertyWithBackingField> _propertiesWithBackingField;

    public AutoPropertyRewriter(SemanticModel semanticModel, IEnumerable<PropertyWithBackingField> propertiesWithBackingField)
    {
        _semanticModel = semanticModel;
        _propertiesWithBackingField = propertiesWithBackingField;
    }

    public override SyntaxNode? VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {
        var propertyWithBackingField = _propertiesWithBackingField.FirstOrDefault(p => p.Property == node);

        if (propertyWithBackingField != null)
        {
            var newProperty = node.WithAccessorList(SyntaxFactory.AccessorList(
                SyntaxFactory.List(new[]
                {
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                })
            ));

            return base.VisitPropertyDeclaration(newProperty);
        }

        return base.VisitPropertyDeclaration(node);
    }

    public override SyntaxNode? VisitFieldDeclaration(FieldDeclarationSyntax node)
    {
        var fieldSymbol = _semanticModel.GetDeclaredSymbol(node.Declaration.Variables.First()) as IFieldSymbol;

        if (_propertiesWithBackingField.Any(p => p.BackingField.Equals(fieldSymbol)))
        {
            return null; // Remove the field
        }

        return base.VisitFieldDeclaration(node);
    }
}