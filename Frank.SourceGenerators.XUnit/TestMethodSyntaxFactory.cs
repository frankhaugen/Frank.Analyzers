using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.Reflection;

namespace Frank.SourceGenerators.XUnit;

public static class TestMethodSyntaxFactory
{
    public static MethodDeclarationSyntax CreateTestMethod(string methodName)
    {
        return SyntaxFactory.MethodDeclaration(
                SyntaxFactory.QualifiedName(
                    SyntaxFactory.QualifiedName(
                        SyntaxFactory.QualifiedName(
                            SyntaxFactory.IdentifierName("System"),
                            SyntaxFactory.IdentifierName("Threading")
                        ),
                        SyntaxFactory.IdentifierName("Tasks")
                    ),
                    SyntaxFactory.IdentifierName("Task")
                ),
                SyntaxFactory.Identifier(methodName)
            )
            .WithAttributeLists(
                SyntaxFactory.SingletonList(
                    SyntaxFactory.AttributeList(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.Attribute(
                                SyntaxFactory.QualifiedName(
                                    SyntaxFactory.IdentifierName("Xunit"),
                                    SyntaxFactory.IdentifierName("Fact")
                                )
                            )
                        )
                    )
                )
            )
            .WithModifiers(
                SyntaxFactory.TokenList(
                    SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                    SyntaxFactory.Token(SyntaxKind.AsyncKeyword)
                )
            )
            .WithBody(
                SyntaxFactory.Block()
                .WithCloseBraceToken(
                    SyntaxFactory.Token(
                        SyntaxFactory.TriviaList(
                            SyntaxFactory.Comment("// Arrange"),
                            SyntaxFactory.Comment("// Act"),
                            SyntaxFactory.Comment("// Assert")
                        ),
                        SyntaxKind.CloseBraceToken,
                        SyntaxFactory.TriviaList()
                    )
                )
            );
    }
    
    public static ClassDeclarationSyntax AddTestMethod(ClassDeclarationSyntax classDeclaration, MethodDeclarationSyntax methodDeclaration)
    {
        return classDeclaration.AddMembers(methodDeclaration);
    }

    public static ClassDeclarationSyntax AddTestMethods(ClassDeclarationSyntax classDeclaration, Type type)
    {
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                          .Where(m => m.DeclaringType != typeof(object));

        foreach (var method in methods)
        {
            var testMethod = CreateTestMethod(method.Name + "Test");
            classDeclaration = AddTestMethod(classDeclaration, testMethod);
        }

        return classDeclaration;
    }
}
