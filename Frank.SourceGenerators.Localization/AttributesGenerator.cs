using Frank.SourceGenerators.Localization.Internals;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Frank.SourceGenerators.Localization;

[Generator(LanguageNames.CSharp)]
public class AttributesGenerator : ISourceGenerator
{
    /// <inheritdoc />
    public void Initialize(GeneratorInitializationContext context)
    {
        // No initialization required for this one
    }

    /// <inheritdoc />
    public void Execute(GeneratorExecutionContext context)
    {
        context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.RootNamespace", out var rootNamespace);
        rootNamespace ??= "Frank.SourceGenerators.Localization";
        
        var nativeNameAttribute = CreateAttributeClass("NativeNameAttribute", "LocalName");
        var englishNameAttribute = CreateAttributeClass("EnglishNameAttribute", "EnglishName");
        var cultureInfoNameAttribute = CreateAttributeClass("CultureInfoNameAttribute", "CultureInfoName");
        
        var nativeNameSourceText = TypeDeclarationHelper.PrepareTypeDeclarationSyntax(nativeNameAttribute, rootNamespace);
        var englishNameSourceText = TypeDeclarationHelper.PrepareTypeDeclarationSyntax(englishNameAttribute, rootNamespace);
        var cultureInfoNameSourceText = TypeDeclarationHelper.PrepareTypeDeclarationSyntax(cultureInfoNameAttribute, rootNamespace);
        
        context.AddSource("NativeNameAttribute.g.cs", nativeNameSourceText);
        context.AddSource("EnglishNameAttribute.g.cs", englishNameSourceText);
        context.AddSource("CultureInfoNameAttribute.g.cs", cultureInfoNameSourceText);
    }
    
    private ClassDeclarationSyntax CreateAttributeClass(string className, string propertyName)
    {
        return SyntaxFactory.ClassDeclaration(className)
            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
            .WithBaseList(SyntaxFactory.BaseList(SyntaxFactory.SingletonSeparatedList<BaseTypeSyntax>(SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName("Attribute")))))
            .WithMembers(SyntaxFactory.List(new MemberDeclarationSyntax[]
            {
                SyntaxFactory.PropertyDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)), propertyName)
                    .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                    .WithAccessorList(SyntaxFactory.AccessorList(SyntaxFactory.SingletonList(SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                    ))),
                SyntaxFactory.ConstructorDeclaration(className)
                    .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                    .WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Parameter(SyntaxFactory.Identifier(char.ToLowerInvariant(propertyName[0]) + propertyName.Substring(1)))
                        .WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword))))))
                    .WithBody(SyntaxFactory.Block(SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, SyntaxFactory.IdentifierName(propertyName), SyntaxFactory.IdentifierName(char.ToLowerInvariant(propertyName[0]) + propertyName.Substring(1))))
                    ))
            }));
    }
}