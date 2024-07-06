using System.Composition;
using Frank.Analyzers.Core.SyntaxRewriters;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Frank.Analyzers.AutoProperties;

[ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(ConvertToAutoPropertyCodeRefactoringProvider)), Shared]
public class ConvertToAutoPropertyCodeRefactoringProvider : CodeRefactoringProvider
{
    public sealed override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
    {
        Document document = context.Document;
        Microsoft.CodeAnalysis.Text.TextSpan textSpan = context.Span;
        CancellationToken cancellationToken = context.CancellationToken;

        SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        SyntaxToken token = root.FindToken(textSpan.Start);
        if (token.Parent == null)
        {
            return;
        }

        PropertyDeclarationSyntax propertyDeclaration = token.Parent.FirstAncestorOrSelf<PropertyDeclarationSyntax>();
            
        if (propertyDeclaration == null)
            return;
            
        if (!HasBothAccessors(propertyDeclaration))
            return;

        // if (!propertyDeclaration.Identifier.Span.IntersectsWith(textSpan.Start))
        //     return;

        context.RegisterRefactoring(
            new ConvertToAutoPropertyCodeAction("Convert to auto property",
                (c) => ConvertToAutoPropertyAsync(document, propertyDeclaration, c)));
    }

    /// <summary>
    /// Returns true if both get and set accessors exist on the given property; otherwise false.
    /// </summary>
    private static bool HasBothAccessors(BasePropertyDeclarationSyntax property)
    {
        SyntaxList<AccessorDeclarationSyntax> accessors = property.AccessorList.Accessors;
        AccessorDeclarationSyntax getter = accessors.FirstOrDefault(ad => ad.Kind() == SyntaxKind.GetAccessorDeclaration);
        AccessorDeclarationSyntax setter = accessors.FirstOrDefault(ad => ad.Kind() == SyntaxKind.SetAccessorDeclaration);

        if (getter != null && setter != null)
        {
            return true;
        }

        return false;
    }

    private async Task<Document> ConvertToAutoPropertyAsync(Document document, PropertyDeclarationSyntax property, CancellationToken cancellationToken)
    {
        SyntaxTree tree = (SyntaxTree)await document.GetSyntaxTreeAsync(cancellationToken).ConfigureAwait(false);
        SemanticModel semanticModel = (SemanticModel)await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

        // Retrieves the get accessor declarations of the specified property.
        AccessorDeclarationSyntax getter = property.AccessorList.Accessors.FirstOrDefault(ad => ad.Kind() == SyntaxKind.GetAccessorDeclaration);

        // Retrieves the type that contains the specified property
        INamedTypeSymbol containingType = semanticModel.GetDeclaredSymbol(property).ContainingType;

        // Find the backing field of the property
        ISymbol backingField = await GetBackingFieldAsync(document, getter, containingType, cancellationToken).ConfigureAwait(false);

        // Rewrite property
        PropertyRewriter propertyRewriter = new PropertyRewriter(semanticModel, backingField, property);
        SyntaxNode root = await tree.GetRootAsync(cancellationToken).ConfigureAwait(false);
        SyntaxNode newRoot = propertyRewriter.Visit(root);

        return document.WithSyntaxRoot(newRoot);
    }

    private async Task<ISymbol> GetBackingFieldAsync(Document document, AccessorDeclarationSyntax getter, INamedTypeSymbol containingType, CancellationToken cancellationToken)
    {
        var isExpressionBody = getter.Body == null && getter.ExpressionBody != null;

        if (isExpressionBody)
        {
            SemanticModel? semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            SymbolInfo symbolInfo = semanticModel.GetSymbolInfo(getter.ExpressionBody.Expression);

            if (symbolInfo.Symbol is IFieldSymbol fieldSymbol && Equals(fieldSymbol.OriginalDefinition.ContainingType, containingType))
            {
                return fieldSymbol;
            }
        }
        else
        {
            SyntaxList<StatementSyntax> statements = getter.Body.Statements;
            if (statements.Count == 1)
            {
                if (statements.FirstOrDefault() is ReturnStatementSyntax returnStatement && returnStatement.Expression != null)
                {
                    SemanticModel? semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
                    SymbolInfo symbolInfo = semanticModel.GetSymbolInfo(returnStatement.Expression);

                    if (symbolInfo.Symbol is IFieldSymbol fieldSymbol && Equals(fieldSymbol.OriginalDefinition.ContainingType, containingType))
                    {
                        return fieldSymbol;
                    }
                }
            }
        }

        return null;
    }

    private class ConvertToAutoPropertyCodeAction : CodeAction
    {
        private readonly Func<CancellationToken, Task<Document>> generateDocument;
        private readonly string title;

        public ConvertToAutoPropertyCodeAction(string title, Func<CancellationToken, Task<Document>> generateDocument)
        {
            this.title = title;
            this.generateDocument = generateDocument;
        }

        public override string Title { get { return title; } }

        protected override Task<Document> GetChangedDocumentAsync(CancellationToken cancellationToken)
        {
            return generateDocument(cancellationToken);
        }
    }
}