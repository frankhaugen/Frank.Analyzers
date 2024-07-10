using System.Globalization;
using Frank.SourceGenerators.Localization.Internals;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Frank.SourceGenerators.Localization;

[Generator(LanguageNames.CSharp)]
public class CountryEnumGenerator : ISourceGenerator
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
        
        var countryEnumSource = GenerateCountryEnum(allRegions);
        
        context.AddSource("CountryEnum.g.cs", TypeDeclarationHelper.PrepareTypeDeclarationSyntax(countryEnumSource, rootNamespace));
    }
    
    private EnumDeclarationSyntax GenerateCountryEnum(IEnumerable<RegionInfo> allRegions)
    {
        var enumMembers = allRegions
            .Select((r, i) => 
                SyntaxFactory
                    .EnumMemberDeclaration(r.TwoLetterISORegionName.ToUpperInvariant())
                    .WithLeadingTrivia(SyntaxFactory.ParseLeadingTrivia($"///<summary>{r.NativeName} ({r.EnglishName})</summary>\n"))
                    .WithEqualsValue(SyntaxFactory.EqualsValueClause(SyntaxFactory.LiteralExpression(
                        SyntaxKind.StringLiteralExpression, 
                        SyntaxFactory.Literal(i)))));
        
        return SyntaxFactory
            .EnumDeclaration("Country")
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .WithLeadingTrivia(SyntaxFactory.ParseLeadingTrivia("///<summary>This enum contains all countries supported by the .NET framework.</summary>\n").ToSyntaxTriviaList())
            .AddMembers(enumMembers.ToArray());
    }
}