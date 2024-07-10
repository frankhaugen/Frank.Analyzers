using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Frank.SourceGenerators.Localization.Internals.Generators;

internal static class DocumentationListGenerator
{
    public static DocumentationCommentTriviaSyntax GenerateBulletListDocumentation(string[] items)
    {
        var listElement = SyntaxFactory.XmlElement(
            SyntaxFactory.XmlElementStartTag(SyntaxFactory.XmlName("list")).WithAttributes(
                SyntaxFactory.SingletonList<XmlAttributeSyntax>(
                    SyntaxFactory.XmlTextAttribute("type", "bullet")
                )),
            SyntaxFactory.List<XmlNodeSyntax>(
                items.Select((item, index) => 
                    SyntaxFactory.XmlElement(
                        SyntaxFactory.XmlElementStartTag(SyntaxFactory.XmlName("item")),
                        SyntaxFactory.SingletonList<XmlNodeSyntax>(
                            SyntaxFactory.XmlElement(
                                SyntaxFactory.XmlElementStartTag(SyntaxFactory.XmlName("description")),
                                SyntaxFactory.SingletonList<XmlNodeSyntax>(SyntaxFactory.XmlText($"Item {index + 1}: {item}")),
                                SyntaxFactory.XmlElementEndTag(SyntaxFactory.XmlName("description"))
                            )
                        ),
                        SyntaxFactory.XmlElementEndTag(SyntaxFactory.XmlName("item"))
                    )).ToArray()
                ),
            SyntaxFactory.XmlElementEndTag(SyntaxFactory.XmlName("list"))
        );
        var documentation = SyntaxFactory.DocumentationComment(listElement);
        return documentation;
    }
}
