using Frank.SourceGenerators.Localization.Internals;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Frank.SourceGenerators.Localization;

[Generator(LanguageNames.CSharp)]
public class ExtensionsGenerator : ISourceGenerator
{
    /// <inheritdoc />
    public void Initialize(GeneratorInitializationContext context)
    {
        // No initialization required for this generator
    }

    /// <inheritdoc />
    public void Execute(GeneratorExecutionContext context)
    {
        context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.RootNamespace", out var rootNamespace);
        rootNamespace ??= "Frank.SourceGenerators.Localization";
        var extensionClass = SyntaxFactory.ClassDeclaration("CultureExtensions")
            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)))
            .WithMembers(SyntaxFactory.List(new MemberDeclarationSyntax[]
            {
                GetExtensionMethodSyntax("GetCultureInfoName", "CultureInfoNameAttribute", "CultureInfoName", "code", "Culture"),
                GetExtensionMethodSyntax("GetNativeName", "NativeNameAttribute", "LocalName", "code", "Culture"),
                GetExtensionMethodSyntax("GetEnglishName", "EnglishNameAttribute", "EnglishName", "code", "Culture")
            }));
        
        var sourceText = TypeDeclarationHelper.PrepareTypeDeclarationSyntax(extensionClass, rootNamespace);
        
        context.AddSource("CultureExtensions.g.cs", sourceText);
    }
    
        public static MethodDeclarationSyntax GetExtensionMethodSyntax(string methodName, string attributeName, string propertyName, string parameterName, string parameterType)
    {
        return SyntaxFactory.MethodDeclaration(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                SyntaxFactory.Identifier(methodName)
            )
            .WithModifiers(
                SyntaxFactory.TokenList(
                    SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                    SyntaxFactory.Token(SyntaxKind.StaticKeyword)
                )
            )
            .WithParameterList(
                SyntaxFactory.ParameterList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Parameter(SyntaxFactory.Identifier(parameterName))
                            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.ThisKeyword)))
                            .WithType(SyntaxFactory.IdentifierName(parameterType))
                    )
                )
            )
            .WithExpressionBody(
                SyntaxFactory.ArrowExpressionClause(
                    SyntaxFactory.BinaryExpression(
                        SyntaxKind.CoalesceExpression,
                        SyntaxFactory.ConditionalAccessExpression(
                            SyntaxFactory.ParenthesizedExpression(
                                SyntaxFactory.BinaryExpression(
                                    SyntaxKind.AsExpression,
                                    SyntaxFactory.ElementAccessExpression(
                                        SyntaxFactory.InvocationExpression(
                                            SyntaxFactory.MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                SyntaxFactory.ElementAccessExpression(
                                                    SyntaxFactory.InvocationExpression(
                                                        SyntaxFactory.MemberAccessExpression(
                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                            SyntaxFactory.InvocationExpression(
                                                                SyntaxFactory.MemberAccessExpression(
                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                    SyntaxFactory.IdentifierName(parameterName),
                                                                    SyntaxFactory.IdentifierName("GetType")
                                                                )
                                                            ),
                                                            SyntaxFactory.IdentifierName("GetMember")
                                                        )
                                                    ).WithArgumentList(
                                                        SyntaxFactory.ArgumentList(
                                                            SyntaxFactory.SingletonSeparatedList(
                                                                SyntaxFactory.Argument(
                                                                    SyntaxFactory.InvocationExpression(
                                                                        SyntaxFactory.MemberAccessExpression(
                                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                                            SyntaxFactory.IdentifierName(parameterName),
                                                                            SyntaxFactory.IdentifierName("ToString")
                                                                        )
                                                                    )
                                                                )
                                                            )
                                                        )
                                                    )
                                                ).WithArgumentList(
                                                    SyntaxFactory.BracketedArgumentList(
                                                        SyntaxFactory.SingletonSeparatedList(
                                                            SyntaxFactory.Argument(
                                                                SyntaxFactory.LiteralExpression(
                                                                    SyntaxKind.NumericLiteralExpression,
                                                                    SyntaxFactory.Literal(0)
                                                                )
                                                            )
                                                        )
                                                    )
                                                ),
                                                SyntaxFactory.IdentifierName("GetCustomAttributes")
                                            )
                                        ).WithArgumentList(
                                            SyntaxFactory.ArgumentList(
                                                SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                                    new SyntaxNodeOrToken[]
                                                    {
                                                        SyntaxFactory.Argument(
                                                            SyntaxFactory.TypeOfExpression(
                                                                SyntaxFactory.IdentifierName(attributeName)
                                                            )
                                                        ),
                                                        SyntaxFactory.Token(SyntaxKind.CommaToken),
                                                        SyntaxFactory.Argument(
                                                            SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression)
                                                        )
                                                    }
                                                )
                                            )
                                        )
                                    ).WithArgumentList(
                                        SyntaxFactory.BracketedArgumentList(
                                            SyntaxFactory.SingletonSeparatedList(
                                                SyntaxFactory.Argument(
                                                    SyntaxFactory.LiteralExpression(
                                                        SyntaxKind.NumericLiteralExpression,
                                                        SyntaxFactory.Literal(0)
                                                    )
                                                )
                                            )
                                        )
                                    ),
                                    SyntaxFactory.IdentifierName(attributeName)
                                )
                            ),
                            SyntaxFactory.MemberBindingExpression(SyntaxFactory.IdentifierName(propertyName))
                        ),
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                            SyntaxFactory.IdentifierName("Empty")
                        )
                    )
                )
            )
            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
    }
    
    public static MethodDeclarationSyntax GenerateMethodXX(string methodName, string attributeName, string propertyName, string parameterName, string parameterType)
    {
        return SyntaxFactory.MethodDeclaration(
                SyntaxFactory.PredefinedType(
                    SyntaxFactory.Token(SyntaxKind.StringKeyword)
                ),
                SyntaxFactory.Identifier(methodName)
            )
            .WithModifiers(
                SyntaxFactory.TokenList(
                    SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                    SyntaxFactory.Token(SyntaxKind.StaticKeyword)
                )
            )
            .WithParameterList(
                SyntaxFactory.ParameterList(
                    SyntaxFactory.SingletonSeparatedList<ParameterSyntax>(
                        SyntaxFactory.Parameter(
                                SyntaxFactory.Identifier(parameterName)
                            )
                            .WithModifiers(
                                SyntaxFactory.TokenList(
                                    SyntaxFactory.Token(SyntaxKind.ThisKeyword)
                                )
                            )
                            .WithType(
                                SyntaxFactory.IdentifierName(parameterType)
                            )
                    )
                )
            )
            .WithExpressionBody(
                SyntaxFactory.ArrowExpressionClause(
                    SyntaxFactory.BinaryExpression(
                        SyntaxKind.CoalesceExpression,
                        SyntaxFactory.ConditionalAccessExpression(
                            SyntaxFactory.ParenthesizedExpression(
                                SyntaxFactory.BinaryExpression(
                                    SyntaxKind.AsExpression,
                                    SyntaxFactory.ElementAccessExpression(
                                        SyntaxFactory.InvocationExpression(
                                            SyntaxFactory.MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                SyntaxFactory.ElementAccessExpression(
                                                    SyntaxFactory.InvocationExpression(
                                                        SyntaxFactory.MemberAccessExpression(
                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                            SyntaxFactory.InvocationExpression(
                                                                SyntaxFactory.MemberAccessExpression(
                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                    SyntaxFactory.IdentifierName(parameterName),
                                                                    SyntaxFactory.IdentifierName("GetType")
                                                                )
                                                            ),
                                                            SyntaxFactory.IdentifierName("GetMember")
                                                        )
                                                    )
                                                    .WithArgumentList(
                                                        SyntaxFactory.ArgumentList(
                                                            SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(
                                                                SyntaxFactory.Argument(
                                                                    SyntaxFactory.InvocationExpression(
                                                                        SyntaxFactory.MemberAccessExpression(
                                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                                            SyntaxFactory.IdentifierName(parameterName),
                                                                            SyntaxFactory.IdentifierName("ToString")
                                                                        )
                                                                    )
                                                                )
                                                            )
                                                        )
                                                    )
                                                ),
                                                SyntaxFactory.IdentifierName("GetCustomAttributes")
                                            )
                                        )
                                        .WithArgumentList(
                                            SyntaxFactory.ArgumentList(
                                                SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                                    new SyntaxNodeOrToken[]
                                                    {
                                                        SyntaxFactory.Argument(
                                                            SyntaxFactory.TypeOfExpression(
                                                                SyntaxFactory.IdentifierName(attributeName)
                                                            )
                                                        ),
                                                        SyntaxFactory.Token(SyntaxKind.CommaToken),
                                                        SyntaxFactory.Argument(
                                                            SyntaxFactory.LiteralExpression(
                                                                SyntaxKind.FalseLiteralExpression
                                                            )
                                                        )
                                                    }
                                                )
                                            )
                                        )
                                    )
                                    .WithArgumentList(
                                        SyntaxFactory.BracketedArgumentList(
                                            SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(
                                                SyntaxFactory.Argument(
                                                    SyntaxFactory.LiteralExpression(
                                                        SyntaxKind.NumericLiteralExpression,
                                                        SyntaxFactory.Literal(0)
                                                    )
                                                )
                                            )
                                        )
                                    ),
                                    SyntaxFactory.IdentifierName(attributeName)
                                )
                            ),
                            SyntaxFactory.MemberBindingExpression(
                                SyntaxFactory.IdentifierName(propertyName)
                            )
                        ),
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.PredefinedType(
                                SyntaxFactory.Token(SyntaxKind.StringKeyword)
                            ),
                            SyntaxFactory.IdentifierName("Empty")
                        )
                    )
                )
            )
            .WithSemicolonToken(
                SyntaxFactory.Token(SyntaxKind.SemicolonToken)
            );
    }
}