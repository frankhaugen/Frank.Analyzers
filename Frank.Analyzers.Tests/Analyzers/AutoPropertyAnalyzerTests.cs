using Frank.Analyzers.AutoProperties;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Xunit.Abstractions;

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
        
        testCode = testCode.ReplaceLineEndings();
        fixedCode = fixedCode.ReplaceLineEndings();
        
        await new CSharpCodeFixTest<AutoPropertySyntaxAnalyzer, AutoPropertyCodeFixProvider, DefaultVerifier>
        {
            TestCode = testCode,
            FixedCode = fixedCode,
            NumberOfIncrementalIterations = 1,
            ExpectedDiagnostics = { DiagnosticResult.CompilerWarning(AutoPropertySyntaxAnalyzer.AutoPropertyRule.Id).WithSpan(6, 9, 10, 10).WithArguments("Name") }
        }.RunAsync();
    }

    [Fact]
    public async Task TestFixerForAll()
    {
        var testCode =
            """
            namespace Frank.Analyzers.Tests.Analyzers
            {
                public class Test
                {
                    private string _name;
                    private int _age;
                    private bool _isActive;
            
                    public string Name
                    {
                        get { return _name; }
                        set { _name = value; }
                    }
            
                    public int Age
                    {
                        get { return _age; }
                        set { _age = value; }
                    }
            
                    public bool IsActive
                    {
                        get { return _isActive; }
                        set { _isActive = value; }
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

                    public int Age { get; set; }

                    public bool IsActive { get; set; }
                }
            }
            """;

        testCode = testCode.ReplaceLineEndings();
        fixedCode = fixedCode.ReplaceLineEndings();

        await new CSharpCodeFixTest<AutoPropertySyntaxAnalyzer, AutoPropertyCodeFixProvider, DefaultVerifier>
        {
            TestCode = testCode,
            FixedCode = fixedCode,
            NumberOfIncrementalIterations = 3,
            ExpectedDiagnostics =
            {
                DiagnosticResult.CompilerWarning(AutoPropertySyntaxAnalyzer.AutoPropertyRule.Id).WithSpan(9, 9, 13, 10).WithArguments("Name"),
                DiagnosticResult.CompilerWarning(AutoPropertySyntaxAnalyzer.AutoPropertyRule.Id).WithSpan(15, 9, 19, 10).WithArguments("Age"),
                DiagnosticResult.CompilerWarning(AutoPropertySyntaxAnalyzer.AutoPropertyRule.Id).WithSpan(21, 9, 25, 10).WithArguments("IsActive")
            }
        }.RunAsync();
    }
}
