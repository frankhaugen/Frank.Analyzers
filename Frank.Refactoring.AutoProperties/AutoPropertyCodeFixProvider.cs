using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Frank.Refactoring.AutoProperties;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AutoPropertyCodeFixProvider)), Shared]
public class AutoPropertyCodeFixProvider : CodeFixProvider
{
    private const string Title = "Use auto property";
    private readonly string Id = AutoPropertySyntaxAnalyzer.AutoPropertyRule.Id;

    public sealed override ImmutableArray<string> FixableDiagnosticIds => [AutoPropertySyntaxAnalyzer.AutoPropertyRule.Id];

    // public sealed override FixAllProvider GetFixAllProvider()
    // {
    //     var scopes = ImmutableArray.Create(FixAllScope.Document, FixAllScope.Project, FixAllScope.Solution);
    //     var provider = FixAllProvider.Create(async (context, document, diagnostics) =>
    //     {
    //         var cancellationToken = context.CancellationToken;
    //         var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
    //         if (root == null)
    //             return document;
    //
    //         var propertyDeclarations = root.DescendantNodes().OfType<PropertyDeclarationSyntax>().ToList();
    //
    //         foreach (var propertyDeclaration in propertyDeclarations)
    //         {
    //             ChangePropertyToAutoPropertyAsync(document, propertyDeclaration, cancellationToken).GetAwaiter().GetResult();
    //         }
    //
    //         return document;
    //     }, scopes);
    //     
    //     return provider;
    // }

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        var propertyDeclaration = root?.FindNode(diagnosticSpan) as PropertyDeclarationSyntax;
        if (propertyDeclaration == null)
            return;
            
        context.RegisterCodeFix(
            CodeAction.Create(
                Title,
                c => ChangePropertyToAutoPropertyAsync(context.Document, propertyDeclaration, c),
                equivalenceKey: Id),
            diagnostic);
    }

    private async Task<Document> ChangePropertyToAutoPropertyAsync(Document document, PropertyDeclarationSyntax propertyDeclaration, CancellationToken cancellationToken)
    {
        var newProperty = propertyDeclaration
            .WithAccessorList(SyntaxFactory.AccessorList(SyntaxFactory.List(new[]
            {
                SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
            })));

        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

        // Modify the root
        var newRoot = root.ReplaceNode(propertyDeclaration, newProperty);

        var fieldDeclarationName = propertyDeclaration.Identifier.Text.ToLowerInvariant();
        var fieldDeclaration = newRoot.DescendantNodes().OfType<FieldDeclarationSyntax>().FirstOrDefault(f => f.Declaration.Variables.Any(v => v.Identifier.Text.Equals($"_{fieldDeclarationName}", StringComparison.OrdinalIgnoreCase)));

        if (fieldDeclaration != null)
        {
            newRoot = newRoot.RemoveNode(fieldDeclaration, SyntaxRemoveOptions.KeepNoTrivia);
        }

        if (newRoot == null)
            throw new InvalidOperationException("Failed to replace property with auto property and remove backing field");
        
        return document.WithSyntaxRoot(newRoot);
    }
}