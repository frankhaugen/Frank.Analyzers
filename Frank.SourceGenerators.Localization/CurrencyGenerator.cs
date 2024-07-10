using Frank.SourceGenerators.Localization.Internals;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Frank.SourceGenerators.Localization;

[Generator(LanguageNames.CSharp)]
public class CurrencyGenerator : ISourceGenerator
{
    /// <inheritdoc />
    public void Initialize(GeneratorInitializationContext context)
    {
        // No initialization required
    }

    /// <inheritdoc />
    public void Execute(GeneratorExecutionContext context)
    {
        context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.RootNamespace", out var rootNamespace);
        rootNamespace ??= "Frank.SourceGenerators.Localization";
        var allCurrencies = RegionHelper.GetAllRegions().Select((r, i) => new Currency(r.CurrencyNativeName, r.CurrencyEnglishName, i, r.ISOCurrencySymbol)).DistinctBy(c => c.Symbol).OrderBy(c => c.Code).ToArray();

        var enumMembers = allCurrencies
            .Select(c =>
                SyntaxFactory
                    .EnumMemberDeclaration(c.Symbol)
                    .WithEqualsValue(SyntaxFactory.EqualsValueClause(SyntaxFactory.LiteralExpression(
                        SyntaxKind.StringLiteralExpression,
                        SyntaxFactory.Literal(c.Code))))
                    .WithLeadingTrivia(SyntaxFactory.ParseLeadingTrivia($"///<summary>{c.Name} ({c.EnglishName})</summary>\n"))
                    .WithTrailingTrivia(SyntaxFactory.LineFeed, SyntaxFactory.LineFeed)
                );
        EnumDeclarationSyntax currencyEnum = SyntaxFactory
            .EnumDeclaration("Currency")
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .WithLeadingTrivia(
                SyntaxFactory.ParseLeadingTrivia($"///<summary>Represents a currency</summary>\n")
                )
            .AddMembers(enumMembers.ToArray());

        
        var sourceText = TypeDeclarationHelper.PrepareTypeDeclarationSyntax(currencyEnum, rootNamespace);
        
        context.AddSource("CurrencyEnum.g.cs", sourceText);
    }
}