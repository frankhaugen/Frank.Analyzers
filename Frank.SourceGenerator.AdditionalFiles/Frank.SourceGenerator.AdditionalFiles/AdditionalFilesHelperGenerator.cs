using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;
using System.Text.RegularExpressions;

namespace Frank.SourceGenerator.AdditionalFiles;

[Generator]
public class AdditionalFilesHelperGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
    }

    public void Execute(GeneratorExecutionContext context)
    {
        GenerateResourceHelperClass(context);
    }

    private static void GenerateResourceHelperClass(GeneratorExecutionContext context)
    {
        const string rootClassName = "AdditionalResources";

        var sourceBuilder = new StringBuilder();
        var additionalFiles = context.AdditionalFiles;

        if (TryGetProjectDir(context, out var projectDir)) return;
        if (TryGetRootNamespace(context, out var rootNamespace)) return;
        
        var root = new ClassMember(rootClassName);
        foreach (var file in additionalFiles)
        {
            var member = GenerateMember(file, projectDir);
            var parent = root.GetOrAddNestedClass(member.Directories);
            parent.MemberDeclarationSyntaxes.Add(member.MemberDeclarationSyntax);
        }

        var rootClass = GenerateClassRecursively(root);
        var compilationUnit = SyntaxFactory.CompilationUnit()
                .AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Reflection")))
                .AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.IO")))
                .AddMembers(SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(rootNamespace)).AddMembers(rootClass))
            ;

        sourceBuilder.Append(compilationUnit.NormalizeWhitespace().ToFullString());

        var sourceString = sourceBuilder.ToString();
        var fileName = rootClassName + ".g.cs";

        var sourceText = SourceText.From(sourceString, Encoding.UTF8);
        context.AddSource(fileName, sourceText);
    }
    
    private static ClassDeclarationSyntax GenerateClassRecursively(ClassMember classMember)
    {
        var className = ConvertPathToClassName(classMember.Name);
        var classDeclaration = SyntaxFactory
            .ClassDeclaration(className)
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword));

        foreach (var nestedClass in classMember.NestedClasses)
        {
            classDeclaration = classDeclaration.AddMembers(GenerateClassRecursively(nestedClass.Value));
        }

        classDeclaration = classDeclaration.AddMembers(classMember.MemberDeclarationSyntaxes.ToArray());

        return classDeclaration;
    }
    
    private static Member GenerateMember(AdditionalText file, string projectDir)
    {
        var relativePath = GetRelativePath(file, projectDir);
        var directoryPath = relativePath.Replace(Path.GetFileName(relativePath), "");
        var directories = directoryPath.Split('/', StringSplitOptions.RemoveEmptyEntries).ToList();
        var memberDeclarationSyntax = GenerateEmbeddedResourceProperty(GetMethodName(file), relativePath);
        
        return new Member
        {
            MemberDeclarationSyntax = memberDeclarationSyntax,
            Directories = directories,
            RelativePath = relativePath
        };
    }

    private struct Member
    {
        public MemberDeclarationSyntax MemberDeclarationSyntax { get; set; }
        public List<string> Directories { get; set; }
        public string RelativePath { get; set; }
    }
    
    private class ClassMember
    {
        public ClassMember(string name) => Name = name;

        public string Name { get; }
        public Dictionary<string, ClassMember> NestedClasses { get; } = new();
        public List<MemberDeclarationSyntax> MemberDeclarationSyntaxes { get; } = new();

        public ClassMember GetOrAddNestedClass(List<string> classPath)
        {
            if (classPath.Count == 0)
            {
                return this;
            }

            var nextClass = classPath[0];
            if (!NestedClasses.TryGetValue(nextClass, out var nextClassMember))
            {
                nextClassMember = new ClassMember(nextClass);
                NestedClasses.Add(nextClass, nextClassMember);
            }

            classPath.RemoveAt(0);
            return nextClassMember.GetOrAddNestedClass(classPath);
        }
    }
    
    private static bool TryGetRootNamespace(GeneratorExecutionContext context, out string? rootNamespace)
    {
        if (!context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.rootnamespace", out rootNamespace))
        {
            context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("FRANK0002", "Failed to generate resource helper class, no root", "Failed to generate resource helper class, no root", "Frank", DiagnosticSeverity.Error, true), Location.None));
            return true;
        }

        return false;
    }

    private static bool TryGetProjectDir(GeneratorExecutionContext context, out string? projectDir)
    {
        if (!context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.projectdir", out projectDir))
        {
            context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("FRANK0001", "Failed to generate resource helper class", "Failed to generate resource helper class", "Frank", DiagnosticSeverity.Error, true), Location.None));
            return true;
        }
        
        return false;
    }

    private static string GetRelativePath(AdditionalText additionalText, string projectPath) => Path.GetRelativePath(projectPath, additionalText.Path).Replace("\\", "/");

    private static PropertyDeclarationSyntax GenerateEmbeddedResourceProperty(string name, string relativeAssetFilePath)
    {
        var syntaxTree = SyntaxFactory
            .PropertyDeclaration(
                SyntaxFactory.ParseTypeName("byte[]"), name)
            .WithExpressionBody(
                SyntaxFactory.ArrowExpressionClause(SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("File"), SyntaxFactory.IdentifierName("ReadAllBytes"))
                    ).WithArgumentList(
                        SyntaxFactory.ArgumentList(
                            SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.Argument(
                                    SyntaxFactory.LiteralExpression(
                                        SyntaxKind.StringLiteralExpression,
                                        SyntaxFactory.Literal(relativeAssetFilePath)
                                    )
                                )
                            )
                        )
                    )
                )
            )
            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)));
        return syntaxTree ;
    }

    private static string ConvertPathToClassName(string path)
    {
        var className = Path.GetFileName(path);
        className = Regex.Replace(className, @"[\W]", "_");
        className = char.ToUpper(className[0]) + (className.Length > 1 ? className.Substring(1) : "");
        return className;
    }

    private static string GetMethodName(AdditionalText additionalText)
    {
        var name = Path.GetFileNameWithoutExtension(additionalText.Path);
        var nameParts = name.Split(".");
        var methodName = nameParts.Last();
        return methodName;
    }
}