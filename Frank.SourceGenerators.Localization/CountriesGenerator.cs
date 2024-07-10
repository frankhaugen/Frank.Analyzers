using System.Globalization;
using Frank.SourceGenerators.Localization.Internals;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Frank.SourceGenerators.Localization;

[Generator(LanguageNames.CSharp)]
public class CountriesGenerator : ISourceGenerator
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
        
        var countrySources = GenerateCountries(allRegions).DistinctBy(x => x.Key).ToList();
        
        foreach (var (name, countrySource) in countrySources)
        {
            context.AddSource($"{name}.g.cs", TypeDeclarationHelper.PrepareTypeDeclarationSyntax(countrySource, rootNamespace));
        }
    }
    
    private IEnumerable<KeyValuePair<string, ClassDeclarationSyntax>> GenerateCountries(IEnumerable<RegionInfo> allRegions)
    {
        var countrySources = new List<KeyValuePair<string, ClassDeclarationSyntax>>();
        foreach (var region in allRegions)
        {
            var countrySource = GenerateCountry(region);
            countrySources.Add(new (Internals.NameHelper.MakeTypeFriendlyName(region.EnglishName), countrySource));
        }

        return countrySources;
    }

    private static ClassDeclarationSyntax GenerateCountry(RegionInfo region) =>
        SyntaxFactory.ClassDeclaration(Internals.NameHelper.MakeTypeFriendlyName(region.EnglishName))
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .AddBaseListTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("ICountryInfo")))
            .AddMembers([
                SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName("string"), "Name")
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                    .AddAccessorListAccessors(SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithExpressionBody(SyntaxFactory.ArrowExpressionClause(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(region.EnglishName)))).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))),
                SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName("Currency"), "CurrencySymbol")
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                    .AddAccessorListAccessors(SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithExpressionBody(SyntaxFactory.ArrowExpressionClause(SyntaxFactory.ParseExpression($"Currency.{region.ISOCurrencySymbol.ToUpperInvariant()};")))),
                SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName("Country"), "CountryCode")
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                    .AddAccessorListAccessors(SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithExpressionBody(SyntaxFactory.ArrowExpressionClause(SyntaxFactory.ParseExpression($"Country.{region.TwoLetterISORegionName.ToUpperInvariant()};")))),
                SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName("Language"), "LanguageCode")
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                    .AddAccessorListAccessors(SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithExpressionBody(SyntaxFactory.ArrowExpressionClause(SyntaxFactory.ParseExpression($"Language.{RegionHelper.GetRegionCultureInfo(region).TwoLetterISOLanguageName.ToUpperInvariant()};")))),
                SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName("System.Globalization.RegionInfo"), "RegionInfo")
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                    .AddAccessorListAccessors(SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithExpressionBody(SyntaxFactory.ArrowExpressionClause(SyntaxFactory.ParseExpression($"new System.Globalization.RegionInfo(\"{region.Name}\")"))).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))),
            ]);
}