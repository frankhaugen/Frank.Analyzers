using System.Collections.Immutable;
using Frank.Analyzers.AutoMapper;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace Frank.Analyzers.Tests.Analyzers;

public class AutomapperAnalyzerTests
{
    private readonly ITestOutputHelper _outputHelper;

    public AutomapperAnalyzerTests(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task Analyze_WhenAutomapperProfileIsMissing_ShouldReturnDiagnosticV3()
    {
        // Arrange
        var code = BaseCode + OrriginalCode;
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var compilation = CSharpCompilation.Create("AnalyzerTest")
            .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
            .AddReferences(MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location))
            .AddSyntaxTrees(syntaxTree);

        var analyzer = new AutoMapperMapAnalyzer();
        var analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(analyzer);


        // Act
        var compilationWithAnalyzers = compilation.WithAnalyzers(analyzers);
        var diagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync();

        foreach (var diagnostic in diagnostics)
        {
            _outputHelper.WriteLine($"{diagnostic.Id}: {diagnostic.GetMessage()} at {diagnostic.Location}");
        }

        // Assert
        Assert.NotEmpty(diagnostics);
    }

    [Fact]
    public async Task Analyze_WhenAutomapperProfileIsMissing_ShouldReturnDiagnosticV1()
    {
        // Arrange
        await new CSharpAnalyzerTest<AutoMapperMapAnalyzer, DefaultVerifier>()
            {
                TestCode = BaseCode + OrriginalCode,
                ReferenceAssemblies = ReferenceAssemblies.Net.Net80.WithPackages([new PackageIdentity("AutoMapper", "13.0.1")]),
                SolutionTransforms =
                {
                    (solution, projectId) =>
                    {
                        var parseOptions = CSharpParseOptions.Default.WithDocumentationMode(DocumentationMode.Diagnose);
                        return solution.WithProjectParseOptions(projectId, parseOptions);
                    }
                },
                DiagnosticVerifier = (diagnostic, result, arg3) => { _outputHelper.WriteLine(diagnostic.ToString()); },
                ExpectedDiagnostics = { DiagnosticResult.CompilerWarning("FR001").WithSpan(7, 5, 11, 6).WithArguments("Name") }
            }
            .RunAsync();

        // Act

        // Assert
    }

    [Fact]
    public async Task Analyze_WhenAutomapperProfileIsMissing_ShouldReturnDiagnostic()
    {
        // Arrange
        await new CSharpCodeFixTest<AutoMapperMapAnalyzer, AutoMapperMapCodeFixProvider, DefaultVerifier>
            {
                TestCode = BaseCode + OrriginalCode,
                FixedCode = BaseCode + ExpectedCode,
                ReferenceAssemblies = ReferenceAssemblies.Net.Net80.WithPackages([new PackageIdentity("AutoMapper", "13.0.1")]),
                SolutionTransforms =
                {
                    (solution, projectId) =>
                    {
                        var parseOptions = CSharpParseOptions.Default.WithDocumentationMode(DocumentationMode.Diagnose);
                        return solution.WithProjectParseOptions(projectId, parseOptions);
                    }
                },
                DiagnosticVerifier = (diagnostic, result, arg3) => { _outputHelper.WriteLine(diagnostic.ToString()); }
                // ExpectedDiagnostics = { DiagnosticResult.CompilerWarning("FR001").WithSpan(7, 5, 11, 6).WithArguments("Name") }
            }
            .RunAsync();

        // Act

        // Assert
    }

    private static string OrriginalCode = """
                                        
                                        public class MyTestingService
                                        {
                                            public void DoSomething()
                                            {
                                                var source = new Source
                                                {
                                                    Name = "Frank",
                                                    Description = "Description",
                                                    Age = 30,
                                                    Address = "Address"
                                                };
                                                
                                                var mapper = new AutoMapper.MapperConfiguration(cfg =>
                                                {
                                                    cfg.AddProfile<MappingProfile>();
                                                }).CreateMapper();
                                                
                                                var destination = mapper.Map<Destination>(source);
                                            }
                                        }
                                        """;
    
    private static string ExpectedCode = """
                                        
                                        public class MyTestingService
                                        {
                                            public void DoSomething()
                                            {
                                                var source = new Source
                                                {
                                                    Name = "Frank",
                                                    Description = "Description",
                                                    Age = 30,
                                                    Address = "Address"
                                                };
                                                
                                                var mapper = new AutoMapper.MapperConfiguration(cfg =>
                                                {
                                                    cfg.AddProfile<MappingProfile>();
                                                }).CreateMapper();
                                                
                                                var destination = mapper.Map<Source, Destination>(source);
                                            }
                                        }
                                        """;
    
    private static string BaseCode = """
                                  using System;
                                  using AutoMapper;
                                  using System.Collections.Generic;
                                  using System.Linq;
              
                                  namespace Frank.Analyzers.Tests.ConsoleApp;
              
                                  public class Source
                                  {
                                    public string Name { get; set; }
                                    
                                    public string? Description { get; set; }
                                    
                                    public int Age { get; set; }
                                    
                                    public string? Address { get; set; }
                                  }
                                  
                                  public class Destination
                                  {
                                      public string Name { get; set; }
                                      
                                      public string? Notes { get; set; }
                                      
                                      public int Age { get; set; }
                                      
                                      public string? Address { get; set; }
                                  }
                                  
                                  public class MappingProfile : Profile
                                  {
                                      public MappingProfile()
                                      {
                                          CreateMap<Source, Destination>()
                                              .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Description));
                                      }
                                  }
                                  
                                  """;
}
