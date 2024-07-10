using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Frank.SourceGenerators.Localization.Internals.Generators;

internal static class AttributeSyntaxFactory
{
    public static AttributeListSyntax AttributeList(params AttributeSyntax[] attributes) =>
        SyntaxFactory.AttributeList(
            SyntaxFactory.SeparatedList(attributes)
        );
    
    public static AttributeSyntax EnglishNameAttribute(string attributeValue) =>
        Attribute("EnglishName", attributeValue);
    
    public static AttributeSyntax NativeNameAttribute(string attributeValue) =>
        Attribute("NativeName", attributeValue);
    
    public static AttributeSyntax DisplayNameAttribute(string attributeValue) =>
        Attribute("DisplayName", attributeValue);
    
    public static AttributeSyntax Attribute(string attributeName, string attributeValue) =>
        SyntaxFactory.Attribute(SyntaxFactory.ParseName(attributeName))
            .WithArgumentList(
                SyntaxFactory.AttributeArgumentList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.AttributeArgument(
                            SyntaxFactory.LiteralExpression(
                                SyntaxKind.StringLiteralExpression,
                                SyntaxFactory.Literal(attributeValue)
                            )
                        )
                    )
                )
            );

}