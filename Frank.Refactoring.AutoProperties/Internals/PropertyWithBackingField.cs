using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Frank.Refactoring.AutoProperties.Internals;

public class PropertyWithBackingField
{
    public IFieldSymbol BackingField { get; }
    public PropertyDeclarationSyntax Property { get; }

    public PropertyWithBackingField(IFieldSymbol backingField, PropertyDeclarationSyntax property)
    {
        BackingField = backingField;
        Property = property;
    }
}