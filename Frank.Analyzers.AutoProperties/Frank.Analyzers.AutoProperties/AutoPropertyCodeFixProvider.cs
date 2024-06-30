using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;

namespace Frank.Analyzers.AutoProperties;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AutoPropertyCodeFixProvider)), Shared]
public class AutoPropertyCodeFixProvider : CodeFixProvider
{
    private static Diagnostic Diagnostic = Diagnostic.Create(DiagnosticProvider.AutoPropertyRule, Location.None);

    public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } = [Diagnostic.Id];

    public override FixAllProvider? GetFixAllProvider()
    {
        return FixAllProvider.Create(Fix);
    }

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var node = root.FindNode(context.Span);
        if (node is not PropertyDeclarationSyntax propertyDeclarationSyntax) return;
        var codeAction = CodeAction.Create("Use auto property", c => Task.FromResult(RenameProperty(context.Document, propertyDeclarationSyntax, c)), Diagnostic.Id);
        
        context.RegisterCodeFix(codeAction, Diagnostic);
    }

    private Document RenameProperty(Document contextDocument, PropertyDeclarationSyntax propertyDeclarationSyntax, CancellationToken cancellationToken)
    {
        var identifierToken = propertyDeclarationSyntax.Identifier;
        var semanticModel = contextDocument.GetSemanticModelAsync(cancellationToken).Result;
        var symbol = semanticModel!.GetDeclaredSymbol(propertyDeclarationSyntax, cancellationToken);
        var newName = identifierToken.Text;
        var newSolution = Renamer.RenameSymbolAsync(contextDocument.Project.Solution, symbol!, newName, contextDocument.Project.Solution.Workspace.Options, cancellationToken).GetAwaiter().GetResult(); 
        return newSolution.GetDocument(contextDocument.Id)!;
    }

    private Task<Document?> Fix(FixAllContext arg1, Document arg2, ImmutableArray<Diagnostic> arg3)
    {
        return null;
    }
}