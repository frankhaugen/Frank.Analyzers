using System.Globalization;
using System.Text;
using Frank.SourceGenerators.Localization.Internals;
using Frank.SourceGenerators.Localization.Internals.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Frank.SourceGenerators.Localization;

[Generator]
public class CultureEnumGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        // Initialization not required
    }

    public void Execute(GeneratorExecutionContext context)
    {
        context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.RootNamespace", out var rootNamespace);
        rootNamespace ??= "Frank.SourceGenerators.Localization";
        
        // Use the syntax receiver to generate the source code
        var cultures = CultureHelper.GetNeutralCultures();
        
        var usings = new[]
        {
            SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("System.Globalization")), SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("System.ComponentModel"))
        };

        var enumMembers = cultures.Select(culture =>
        {
            var enumMember = SyntaxFactory
                .EnumMemberDeclaration(SyntaxFactory.Identifier(culture.TwoLetterISOLanguageName.ToUpperInvariant()))
                .WithEqualsValue(SyntaxFactory.EqualsValueClause(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(culture.LCID))))
                .WithAttributeLists([
                    AttributeSyntaxFactory.AttributeList(AttributeSyntaxFactory.Attribute("NativeName", culture.NativeName)), 
                    AttributeSyntaxFactory.AttributeList(AttributeSyntaxFactory.Attribute("EnglishName", culture.EnglishName)),
                    AttributeSyntaxFactory.AttributeList(AttributeSyntaxFactory.Attribute("Description", culture.DisplayName))
                ])
                .WithLeadingTrivia(GetDocumentationTrivia(culture))
                .WithTrailingTrivia(SyntaxFactory.LineFeed, SyntaxFactory.LineFeed);
            return enumMember;
        }).ToArray();

        var enumDeclaration = SyntaxFactory.EnumDeclaration("Culture")
            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
            .WithMembers(SyntaxFactory.SeparatedList(enumMembers));

        var sourceText = TypeDeclarationHelper.PrepareTypeDeclarationSyntax(enumDeclaration, rootNamespace);
        
        context.AddSource("CultureEnum.g.cs", sourceText);
    }

    private SyntaxTriviaList GetDocumentationTrivia(CultureInfo culture)
    {
        var documentationCommentTriviaSyntax = SyntaxFactory.DocumentationCommentTrivia(SyntaxKind.MultiLineDocumentationCommentTrivia, SyntaxFactory.List(new XmlNodeSyntax[]
                {
                    SyntaxFactory.XmlText("/// "),
                    SyntaxFactory.XmlSummaryElement(SyntaxFactory.XmlText($"{culture.NativeName} ({culture.EnglishName})")),
                    SyntaxFactory.XmlText("\n"),
                    SyntaxFactory.XmlNewLine(""),
                    SyntaxFactory.XmlRemarksElement(SyntaxFactory.XmlText($"{culture.Name} ({culture.LCID})")),
                    SyntaxFactory.XmlText("\n"),
                }
            )
        );
        
        var syntaxTriviaList = new SyntaxTriviaList();
        
        syntaxTriviaList = syntaxTriviaList.Add(SyntaxFactory.Trivia(documentationCommentTriviaSyntax));

        return syntaxTriviaList;
    }
}