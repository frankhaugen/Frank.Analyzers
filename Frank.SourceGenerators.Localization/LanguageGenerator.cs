using Frank.SourceGenerators.Localization.Internals;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Frank.SourceGenerators.Localization;

[Generator(LanguageNames.CSharp)]
public class LanguageGenerator : ISourceGenerator
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
        var allLanguages = CultureHelper.GetAllCultures().Select(c => new Language(c.TwoLetterISOLanguageName.ToUpperInvariant(), c.LCID, c.NativeName, c.EnglishName)).DistinctBy(l => l.Name).OrderBy(l => l.Code).ToArray();

        var enumMembers = allLanguages
            .Select(l => 
                SyntaxFactory
                    .EnumMemberDeclaration(l.Name)
                    .WithLeadingTrivia(SyntaxFactory.ParseLeadingTrivia($"///<summary>{l.NativeName} ({l.EnglishName})</summary>\n"))
                    .WithEqualsValue(SyntaxFactory.EqualsValueClause(SyntaxFactory.LiteralExpression(
                        SyntaxKind.StringLiteralExpression, 
                        SyntaxFactory.Literal(l.Code)))));
        
        EnumDeclarationSyntax languageEnum = SyntaxFactory
            .EnumDeclaration("Language")
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .WithLeadingTrivia(SyntaxFactory.ParseLeadingTrivia("///<summary>This enum contains all languages supported by the .NET framework.</summary>\n").ToSyntaxTriviaList())
            .AddMembers(enumMembers.ToArray());

        var sourceText = TypeDeclarationHelper.PrepareTypeDeclarationSyntax(languageEnum, rootNamespace);
        
        context.AddSource("LanguageEnum.g.cs", sourceText);
    }
}