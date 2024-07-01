using Frank.Analyzers.AutoProperties;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Xunit.Abstractions;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<
    Frank.Analyzers.AutoProperties.AutoPropertySyntaxAnalyzer,
    Frank.Analyzers.AutoProperties.AutoPropertyCodeFixProvider>;

namespace Frank.Analyzers.Tests.Analyzers;

public class AutoPropertyAnalyzerTests
{
    private readonly ITestOutputHelper _outputHelper;

    public AutoPropertyAnalyzerTests(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task TestAnalyzer()
    {
        var code =
            """
            namespace Frank.Analyzers.Tests.Analyzers;

            public class Test
            {
                private string _name;
            
                public string Name
                {
                    get { return _name; }
                    set { _name = value; }
                }
            }
            """;
        await new CSharpAnalyzerTest<AutoPropertySyntaxAnalyzer, DefaultVerifier>
        {
            TestCode = code,
            SolutionTransforms =
            {
                (solution, projectId) =>
                {
                    CSharpParseOptions parseOptions = CSharpParseOptions.Default.WithDocumentationMode(DocumentationMode.Diagnose);
                    return solution.WithProjectParseOptions(projectId, parseOptions);
                },
            },
            DiagnosticVerifier = (diagnostic, result, arg3) =>
            {
                _outputHelper.WriteLine(diagnostic.ToString());
            },
            ExpectedDiagnostics = { DiagnosticResult.CompilerWarning("FR001").WithSpan(7, 5, 11, 6).WithArguments("Name") }
        }.RunAsync();
    }
    
    [Fact]
    public async Task TestFixer()
    {
        var testCode =
            """
            namespace Frank.Analyzers.Tests.Analyzers
            {
                public class Test
                {
                    private string _name;
            
                    public string Name
                    {
                        get { return _name; }
                        set { _name = value; }
                    }
                }
            }
            """;

        var fixedCode =
            """
            namespace Frank.Analyzers.Tests.Analyzers
            {
                public class Test
                {
                    public string Name { get; set; }
                }
            }
            """;
        
        await new CSharpCodeFixTest<AutoPropertySyntaxAnalyzer, AutoPropertyCodeFixProvider, DefaultVerifier>
        {
            TestCode = testCode,
            FixedCode = fixedCode,
            NumberOfIncrementalIterations = 1,
            ExpectedDiagnostics = { DiagnosticResult.CompilerWarning("FR001").WithSpan(7, 9, 11, 10).WithArguments("Name") }
        }.RunAsync();
    }
}
