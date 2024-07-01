using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Frank.Analyzers.AutoProperties
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AutoPropertyCodeFixProvider)), Shared]
    public class AutoPropertyCodeFixProvider : CodeFixProvider
    {
        private const string Title = "Use auto property";

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
        //         var newRoot = root;
        //
        //         foreach (var diagnostic in diagnostics)
        //         {
        //             var diagnosticSpan = diagnostic.Location.SourceSpan;
        //
        //             var propertyDeclaration = newRoot?.FindNode(diagnosticSpan) as PropertyDeclarationSyntax;
        //             var newProperty = propertyDeclaration?
        //                 .WithAccessorList(null)
        //                 .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        //
        //             newRoot = newRoot?.ReplaceNode(propertyDeclaration, newProperty);
        //         }
        //
        //         var newDocument = document.WithSyntaxRoot(newRoot ?? throw new InvalidOperationException("Failed to replace property"));
        //         return newDocument;
        //     }, scopes);
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
                    equivalenceKey: Title),
                diagnostic);
        }

        private async Task<Document> ChangePropertyToAutoPropertyAsync(Document document, PropertyDeclarationSyntax propertyDeclaration, CancellationToken cancellationToken)
        {
            var newProperty = propertyDeclaration
                .WithAccessorList(null)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var newRoot = root?.ReplaceNode(propertyDeclaration, newProperty);

            return document.WithSyntaxRoot(newRoot ?? throw new InvalidOperationException("Failed to replace property"));
        }
    }
}
