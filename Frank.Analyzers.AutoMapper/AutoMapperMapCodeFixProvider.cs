using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Frank.Analyzers.AutoMapper;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AutoMapperMapCodeFixProvider)), Shared]
public class AutoMapperMapCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = 
        ImmutableArray.Create(DiagnosticDescriptors.AutoMapperMap.Id);

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        var invocation = root.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().First();

        if (invocation == null)
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                title: "Add missing type argument",
                createChangedDocument: c => AddMissingTypeArgumentAsync(context.Document, invocation, c),
                equivalenceKey: nameof(AutoMapperMapCodeFixProvider)),
            diagnostic);
    }

    private async Task<Document> AddMissingTypeArgumentAsync(Document document, InvocationExpressionSyntax invocation, CancellationToken cancellationToken)
    {
        var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
        
        // Extract the type of the argument passed to Map()
        var argument = invocation.ArgumentList.Arguments.FirstOrDefault();
        var argumentType = semanticModel.GetTypeInfo(argument.Expression, cancellationToken).Type;

        var genericName = (GenericNameSyntax)invocation.Expression;

        // Construct a new generic type argument list with the inferred source type
        var newGenericName = genericName.WithTypeArgumentList(
            SyntaxFactory.TypeArgumentList(
                SyntaxFactory.SeparatedList<TypeSyntax>(
                    new SyntaxNodeOrToken[]
                    {
                        genericName.TypeArgumentList.Arguments.First(), // The existing Destination type
                        SyntaxFactory.Token(SyntaxKind.CommaToken),
                        SyntaxFactory.ParseTypeName(argumentType.ToDisplayString()) // Inferred Source type
                    }
                )
            )
        );

        var newInvocation = invocation.WithExpression(newGenericName);
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        var newRoot = root.ReplaceNode(invocation, newInvocation);

        return document.WithSyntaxRoot(newRoot);
    }
}
