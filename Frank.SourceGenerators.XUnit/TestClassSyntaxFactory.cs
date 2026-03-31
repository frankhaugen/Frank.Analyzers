using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Frank.SourceGenerators.XUnit;

public static class TestClassSyntaxFactory
{
    public static CompilationUnitSyntax CreateTestClass(string namespaceName, string typeName)
    {
        var classDeclaration = CreateClassDeclaration(typeName);
        var namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(namespaceName))
                                               .WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(classDeclaration));
        
        return SyntaxFactory.CompilationUnit().WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(namespaceDeclaration));
    }

    public static CompilationUnitSyntax CreateTestClass(Type type)
    {
        var namespaceName = type.Namespace;
        var typeName = type.Name + "Tests";
        return CreateTestClass(namespaceName, typeName);
    }

    public static ClassDeclarationSyntax CreateClassDeclaration(string className)
    {
        return SyntaxFactory.ClassDeclaration(className)
                            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                            .WithMembers(SyntaxFactory.List(new MemberDeclarationSyntax[]
                            {
                                CreateFieldDeclaration(),
                                CreateConstructorDeclaration(className)
                            }));
    }

    private static FieldDeclarationSyntax CreateFieldDeclaration()
    {
        return SyntaxFactory.FieldDeclaration(
                SyntaxFactory.VariableDeclaration(
                    SyntaxFactory.QualifiedName(
                        SyntaxFactory.QualifiedName(
                            SyntaxFactory.IdentifierName("Xunit"),
                            SyntaxFactory.IdentifierName("Abstractions")
                        ),
                        SyntaxFactory.IdentifierName("ITestOutputHelper")
                    )
                )
                .WithVariables(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.VariableDeclarator("_outputHelper")
                    )
                )
            )
            .WithModifiers(
                SyntaxFactory.TokenList(
                    SyntaxFactory.Token(SyntaxKind.PrivateKeyword),
                    SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)
                )
            );
    }

    private static ConstructorDeclarationSyntax CreateConstructorDeclaration(string className)
    {
        return SyntaxFactory.ConstructorDeclaration(className)
            .WithModifiers(
                SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            )
            .WithParameterList(
                SyntaxFactory.ParameterList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Parameter(SyntaxFactory.Identifier("outputHelper"))
                        .WithType(
                            SyntaxFactory.QualifiedName(
                                SyntaxFactory.QualifiedName(
                                    SyntaxFactory.IdentifierName("Xunit"),
                                    SyntaxFactory.IdentifierName("Abstractions")
                                ),
                                SyntaxFactory.IdentifierName("ITestOutputHelper")
                            )
                        )
                    )
                )
            )
            .WithBody(
                SyntaxFactory.Block(
                    SyntaxFactory.SingletonList<StatementSyntax>(
                        SyntaxFactory.ExpressionStatement(
                            SyntaxFactory.AssignmentExpression(
                                SyntaxKind.SimpleAssignmentExpression,
                                SyntaxFactory.IdentifierName("_outputHelper"),
                                SyntaxFactory.IdentifierName("outputHelper")
                            )
                        )
                    )
                )
            );
    }
}
