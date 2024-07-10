using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Frank.SourceGenerators.Localization.Internals.Generators;

internal static class DocumentationSyntaxFactory
{
    public static List<string> CreateDocumentationTrivia(string? summary = null, string? remarks = null, params string[]? list)
    {
        var xmlDocsBuilder = new StringBuilder();
        
        if (summary is not null)
        {
            xmlDocsBuilder.AppendLine($"<summary>{summary}</summary>");
            // documentationCommentTriviaList = documentationCommentTriviaList.Add(SyntaxFactory.DocumentationCommentTrivia(SyntaxKind.MultiLineDocumentationCommentTrivia, SyntaxFactory.List(new XmlNodeSyntax[]
            // {
            //     SyntaxFactory.XmlElement("summary", [SyntaxFactory.XmlText(summary)])
            // })));
        }
        
        if (remarks is not null)
        {
            xmlDocsBuilder.AppendLine($"<remarks>{remarks}</remarks>");
            // documentationCommentTriviaList = documentationCommentTriviaList.Add(SyntaxFactory.DocumentationCommentTrivia(SyntaxKind.MultiLineDocumentationCommentTrivia, SyntaxFactory.List(new XmlNodeSyntax[]
            // {
            //     SyntaxFactory.XmlElement("remarks", [SyntaxFactory.XmlText(remarks)])
            // })));
        }

        if (list is { Length: > 0 })
        {
            xmlDocsBuilder.AppendLine("<list type=\"bullet\">");
            foreach (var item in list)
            {
                xmlDocsBuilder.AppendLine($"<item><description>{item}</description></item>");
            }
            xmlDocsBuilder.AppendLine("</list>");
            // documentationCommentTriviaList = documentationCommentTriviaList.Add(DocumentationListGenerator.GenerateBulletListDocumentation(list));
        }
        
        // add newlines between documentation and code
        // documentationCommentTriviaList.Add(SyntaxFactory.DocumentationCommentTrivia(SyntaxKind.MultiLineDocumentationCommentTrivia, SyntaxFactory.List(new XmlNodeSyntax[]
        // {
        //     SyntaxFactory.XmlText("\n")
        // })));
        
        return xmlDocsBuilder.ToString().ReplaceLineEndings("\n").Split("\n").ToList();
    }
}