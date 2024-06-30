using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Frank.Analyzers.AutoProperties;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AutoPropertyCodeFixProvider)), Shared]
public class AutoPropertyCodeFixProvider : CodeFixProvider
{
    private static readonly Diagnostic Diagnostic = Diagnostic.Create(DiagnosticProvider.AutoPropertyRule, Location.None);

    public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } = [Diagnostic.Id];
    
    public override FixAllProvider? GetFixAllProvider()
    {
        return FixAllProvider.Create(Fix);
    }

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var node = root?.FindNode(context.Span);
        if (node is not PropertyDeclarationSyntax propertyDeclarationSyntax) return;
        var document = context.Document;
        var codeAction = CodeAction.Create("Use auto property", async c => await ChangePropertyToAutoPropertyAsync(document, propertyDeclarationSyntax, c), Diagnostic.Id);
        
        context.RegisterCodeFix(codeAction, Diagnostic);
    }

    private async Task<Document> ChangePropertyToAutoPropertyAsync(Document document, PropertyDeclarationSyntax propertyDeclarationSyntax, CancellationToken cancellationToken)
    {
        var newProperty = propertyDeclarationSyntax.WithAccessorList(null).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        var root = await document.GetSyntaxRootAsync(cancellationToken);
        var newRoot = root?.ReplaceNode(propertyDeclarationSyntax, newProperty);
        return document.WithSyntaxRoot(newRoot ?? throw new InvalidOperationException($"Failed to replace {propertyDeclarationSyntax} with {newProperty}"));
    }

    private async Task<Document?> Fix(FixAllContext arg1, Document arg2, ImmutableArray<Diagnostic> arg3)
    {
        var root = await arg2.GetSyntaxRootAsync(arg1.CancellationToken).ConfigureAwait(false);
        var properties = arg3.Select(d => root?.FindNode(d.Location.SourceSpan)).OfType<PropertyDeclarationSyntax>().ToArray();
        if (properties.Length == 0) return arg2;
        var newRoot = properties.Aggregate(root, (current, property) => current?.ReplaceNode(property, property.WithAccessorList(null).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))));
        return arg2.WithSyntaxRoot(newRoot ?? throw new InvalidOperationException("Failed to replace properties"));
    }
}