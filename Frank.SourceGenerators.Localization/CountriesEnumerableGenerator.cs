using System.Globalization;
using Frank.SourceGenerators.Localization.Internals;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Frank.SourceGenerators.Localization;

[Generator(LanguageNames.CSharp)]
public class CountriesEnumerableGenerator : ISourceGenerator
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
        
        var allRegions = RegionHelper
            .GetAllRegions()
            ;
        
        var enumerableSource = GenerateEnumerable(allRegions);
        
        context.AddSource("Countries.g.cs", TypeDeclarationHelper.PrepareTypeDeclarationSyntax(enumerableSource, rootNamespace));
    }
    
    private ClassDeclarationSyntax GenerateEnumerable(RegionInfo[] allRegions)
    {
        var staticInstanceProperty = SyntaxFactory.PropertyDeclaration(SyntaxFactory.IdentifierName("Countries"), SyntaxFactory.Identifier("Instance"))
            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)))
            .WithInitializer(SyntaxFactory.EqualsValueClause(
                SyntaxFactory.ObjectCreationExpression(SyntaxFactory.IdentifierName("Countries"))
                    .WithArgumentList(SyntaxFactory.ArgumentList())
            ))
            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
            .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);
        
        var indexerMethod = SyntaxFactory.IndexerDeclaration(SyntaxFactory.IdentifierName("ICountryInfo"))
            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
            .WithParameterList(SyntaxFactory.BracketedParameterList(
                SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("index"))
                        .WithType(SyntaxFactory.ParseTypeName("Country"))
                )
            ))
            .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(
                SyntaxFactory.InvocationExpression(SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName("Instance"),
                    SyntaxFactory.IdentifierName("First")
                ))
                .WithArgumentList(
                    SyntaxFactory.ArgumentList(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.Argument(
                                SyntaxFactory.SimpleLambdaExpression(
                                    SyntaxFactory.Parameter(
                                        SyntaxFactory.Identifier("x")
                                    )
                                ).WithExpressionBody(
                                    SyntaxFactory.BinaryExpression(
                                        SyntaxKind.EqualsExpression,
                                        SyntaxFactory.MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            SyntaxFactory.IdentifierName("x"),
                                            SyntaxFactory.IdentifierName("CountryCode")
                                        ),
                                        SyntaxFactory.IdentifierName("index")
                                    )
                                )
                            )
                        )
                    )
                )
            ))
            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
            .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed)
            ;

        var getEnumeratorMethod = SyntaxFactory.MethodDeclaration(
                SyntaxFactory.GenericName("IEnumerator")
                    .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(
                        SyntaxFactory.SingletonSeparatedList<TypeSyntax>(SyntaxFactory.IdentifierName("ICountryInfo"))
                    )),
                SyntaxFactory.Identifier("GetEnumerator"))
            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
            .WithBody(SyntaxFactory.Block(
                allRegions.Select(region => SyntaxFactory.YieldStatement(SyntaxKind.YieldReturnStatement,
                    SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName($"Get{NameHelper.MakeTypeFriendlyName(region.EnglishName)}"))
                        .WithArgumentList(SyntaxFactory.ArgumentList())
                    .WithArgumentList(SyntaxFactory.ArgumentList())
                ))
            ));

        var countryMethods = allRegions.DistinctBy(x => x.EnglishName).Select(region =>
        {
            var methodName = $"Get{NameHelper.MakeTypeFriendlyName(region.EnglishName)}";
            return SyntaxFactory.MethodDeclaration(SyntaxFactory.IdentifierName("ICountryInfo"), SyntaxFactory.Identifier(methodName))
                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)))
                .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(
                    SyntaxFactory.ObjectCreationExpression(SyntaxFactory.IdentifierName(NameHelper.MakeTypeFriendlyName(region.EnglishName)))
                        .WithArgumentList(SyntaxFactory.ArgumentList())
                ))
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        }).ToArray();

        var explicitGetEnumeratorMethod = SyntaxFactory.MethodDeclaration(SyntaxFactory.IdentifierName("IEnumerator"), SyntaxFactory.Identifier("GetEnumerator"))
            .WithExplicitInterfaceSpecifier(SyntaxFactory.ExplicitInterfaceSpecifier(SyntaxFactory.IdentifierName("IEnumerable")))
            .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(
                SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName("GetEnumerator"))
            ))
            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

        return SyntaxFactory.ClassDeclaration("Countries")
            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
            .WithBaseList(SyntaxFactory.BaseList(SyntaxFactory.SingletonSeparatedList<BaseTypeSyntax>(
                SyntaxFactory.SimpleBaseType(SyntaxFactory.GenericName("IEnumerable")
                    .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(
                        SyntaxFactory.SingletonSeparatedList<TypeSyntax>(SyntaxFactory.IdentifierName("ICountryInfo"))
                    ))
                ))
            ))
            .WithMembers(SyntaxList.Create<MemberDeclarationSyntax>(
                new MemberDeclarationSyntax[]
                {
                    staticInstanceProperty,
                    indexerMethod,
                    explicitGetEnumeratorMethod,
                    getEnumeratorMethod,
                }.Concat(countryMethods).ToArray()
            ));
    }

}