using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Frank.SourceGenerators.Localization.Internals;

internal static class AnythingExtensions
{
    public static T[] AsArray<T>(this T item) => [item];


    public static SyntaxTriviaList ToSyntaxTrivia(this DocumentationCommentTriviaSyntax documentationCommentTriviaSyntax)
    {
        var triviaList = new SyntaxTriviaList();
        triviaList = triviaList.Add(SyntaxFactory.DocumentationCommentExterior("///"));
        triviaList = triviaList.Add(SyntaxFactory.DocumentationCommentExterior(" "));
        triviaList = triviaList.Add(SyntaxFactory.DocumentationCommentExterior(documentationCommentTriviaSyntax.NormalizeWhitespace().ToFullString()));
        triviaList = triviaList.Add(SyntaxFactory.DocumentationCommentExterior("\n"));
        triviaList = triviaList.Add(SyntaxFactory.DocumentationCommentExterior("\n"));
        return triviaList;
    }
}