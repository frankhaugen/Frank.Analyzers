using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Frank.SourceGenerators.XUnit;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public abstract class AutoGenerateTestsAttribute : Attribute
{
    public Type Type { get; }
    
    protected AutoGenerateTestsAttribute(Type type)
    {
        Type = type;
    }
}

/// <summary>
/// Generates tests for the specified type.
/// </summary>
/// <typeparam name="T"></typeparam>
public class AutoGenerateTestsAttribute<T> : AutoGenerateTestsAttribute where T : class
{
    /// <inheritdoc />
    public AutoGenerateTestsAttribute() : base(typeof(T))
    {
    }
}

/// <summary>
/// Generates tests for the specified type.
/// </summary>
[Generator(LanguageNames.CSharp)]
public class AutoGenerateTestsGenerator : ISourceGenerator
{
    
    public void Initialize(GeneratorInitializationContext context) =>
        Types.AddRange(Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(x => x.IsGenericType && x.GetCustomAttributes<AutoGenerateTestsAttribute>().Any())
            .Select(type => type.GetCustomAttribute<AutoGenerateTestsAttribute>())
            .Select(attribute => attribute?.Type)
            .OfType<Type>());

    private List<Type> Types { get; } = new();

    public void Execute(GeneratorExecutionContext context)
    {
        foreach (var type in Types)
        {
            var testClass = TestClassSyntaxFactory.CreateTestClass(type);
            var sourceText = SourceText.From(testClass.NormalizeWhitespace().ToFullString(), Encoding.UTF8);
            context.AddSource($"{type.Name}Tests.cs", sourceText);
        }
    }
}
    