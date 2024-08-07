﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace Frank.Refactoring.AutoProperties.Internals;

public class PropertyRewriter : CSharpSyntaxRewriter
{
    private readonly ISymbol backingField;
    private readonly PropertyDeclarationSyntax property;
    private readonly SemanticModel semanticModel;

    public PropertyRewriter(SemanticModel semanticModel, ISymbol backingField, PropertyDeclarationSyntax property)
    {
        this.semanticModel = semanticModel;
        this.backingField = backingField;
        this.property = property;
    }

    public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax name)
    {
        if (backingField != null)
            if (name.Identifier.ValueText.Equals(backingField.Name))
            {
                var symbolInfo = semanticModel.GetSymbolInfo(name);

                // Check binding info
                if (symbolInfo.Symbol != null &&
                    Equals(symbolInfo.Symbol.OriginalDefinition, backingField))
                {
                    name = name.WithIdentifier(
                        SyntaxFactory.Identifier(property.Identifier.ValueText));

                    return name.WithAdditionalAnnotations(Formatter.Annotation);
                }
            }

        return name;
    }

    public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax propertyDeclaration)
    {
        if (propertyDeclaration == property)
            // Add an annotation to format the new property.
            return ConvertToAutoProperty(propertyDeclaration).WithAdditionalAnnotations(Formatter.Annotation);

        return base.VisitPropertyDeclaration(propertyDeclaration);
    }

    public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax field)
    {
        // Retrieve the symbol for the field's variable
        if (field.Declaration.Variables.Count == 1)
            if (Equals(semanticModel.GetDeclaredSymbol(field.Declaration.Variables.First()), backingField))
                return null;

        return field;
    }

    public override SyntaxNode VisitVariableDeclarator(VariableDeclaratorSyntax variable)
    {
        // Retrieve the symbol for the variable declarator
        if (variable.Parent.Parent is FieldDeclarationSyntax field && field.Declaration.Variables.Count == 1)
            if (Equals(semanticModel.GetDeclaredSymbol(variable), backingField))
                return null;

        return variable;
    }

    private PropertyDeclarationSyntax ConvertToAutoProperty(PropertyDeclarationSyntax propertyDeclaration)
    {
        // Produce the new property.
        var newProperty = property
            .WithAccessorList(
                SyntaxFactory.AccessorList(
                    SyntaxFactory.List(new[]
                    {
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                    })));

        return newProperty;
    }
}