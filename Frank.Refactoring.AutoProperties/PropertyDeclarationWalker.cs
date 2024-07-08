using Frank.Refactoring.AutoProperties.Internals;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Frank.Refactoring.AutoProperties;

public class PropertyDeclarationWalker(SemanticModel semanticModel) : CSharpSyntaxWalker
{
    public List<PropertyWithBackingField> PropertiesWithBackingFields { get; private set;  } = [];

    /// <inheritdoc />
    public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {
        var propertyWithBackingField = ExtractPropertiesWithBackingField(node);
        if (propertyWithBackingField != null)
            PropertiesWithBackingFields.Add(propertyWithBackingField);
        base.VisitPropertyDeclaration(node);
    }
    
    private PropertyWithBackingField? ExtractPropertiesWithBackingField(PropertyDeclarationSyntax propertyDeclaration)
    {
        if (propertyDeclaration.AccessorList == null || propertyDeclaration.AccessorList.Accessors.Count != 2)
            return null;
        
        var backingField = GetBackingFieldSymbol(propertyDeclaration);
        if (backingField == null)
            return null;
        
        return new PropertyWithBackingField(backingField, propertyDeclaration);
    }

    private IFieldSymbol? GetBackingFieldSymbol(PropertyDeclarationSyntax property)
    {
        var propertySymbol = semanticModel.GetDeclaredSymbol(property);
        if (propertySymbol == null) return null;

        foreach (var accessor in propertySymbol.GetMethod?.DeclaringSyntaxReferences.Concat(propertySymbol.SetMethod?.DeclaringSyntaxReferences) ?? Enumerable.Empty<SyntaxReference>())
        {
            var syntaxNode = accessor.GetSyntax() as AccessorDeclarationSyntax;
            if (syntaxNode == null) continue;

            foreach (var identifier in syntaxNode.DescendantNodes().OfType<IdentifierNameSyntax>())
            {
                var fieldSymbol = semanticModel.GetSymbolInfo(identifier).Symbol as IFieldSymbol;
                if (fieldSymbol != null && fieldSymbol.ContainingType.Equals(propertySymbol.ContainingType))
                    return fieldSymbol;
            }
        }

        return null;
    }
}

