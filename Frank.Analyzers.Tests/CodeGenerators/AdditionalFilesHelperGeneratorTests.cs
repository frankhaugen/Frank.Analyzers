using System.Reflection;
using System.Text;
using Frank.Analyzers.Tests.TestingInfrastructure;
using Frank.SourceGenerator.AdditionalFiles;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;
using static System.Collections.Immutable.ImmutableArray<Frank.Analyzers.Tests.TestingInfrastructure.AnalyzerAdditionalText>;

namespace Frank.Analyzers.Tests.CodeGenerators;

public class AdditionalFilesHelperGeneratorTests : CSharpSourceGeneratorTest<AdditionalFilesHelperGenerator, DefaultVerifier>
{
    private readonly ITestOutputHelper _outputHelper;

    public AdditionalFilesHelperGeneratorTests(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task Test()
    {
        TestState.AdditionalFiles.Add(("Assets/Teapot.obj", SourceText.From("Hello world", Encoding.UTF8)));
        TestState.Sources.Add(@"
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Text;
            using System.Threading.Tasks;
            
            namespace Frank.Refactoring.AutoProperties
            {
                public class AutoPropertyRefactor
                {
                    private string _name;
                    
                    public string Name
                    {
                        get => _name;
                        set => _name = value;
                    }
                }
            }");

        TestState.GeneratedSources.Add(("Frank.SourceGenerator.AdditionalFiles\\Frank.SourceGenerator.AdditionalFiles.AdditionalFilesHelperGenerator\\AdditionalFilesHelper.g.cs", SourceText.From(
            """
            using System.Reflection;
            using System.IO;

            namespace TestProject
            {
                public static class AdditionalFilesHelper
                {
                    public static class Assets
                    {
                        /// <summary>FileInfo for TeapotFileInfo with extension .obj</summary>
                        public static FileInfo TeapotFileInfo => new FileInfo("Assets/Teapot.obj");
                    }
                }
            }
            """,
            Encoding.UTF8
            )));
        
        await RunAsync();
        
        _outputHelper.WriteLine(string.Join(Environment.NewLine, TestState.GeneratedSources.Select(x => x.Item2.ToString())));
    }

    /// <inheritdoc />
    protected override AnalyzerOptions GetAnalyzerOptions(Project project)
    {
        var provider = new TestOptionsProvider();
        return new AnalyzerOptions([], provider);
    }

    [Fact]
    public void Use()
    {
        // var thing = AdditionalFilesHelper.CodeGenerators.TestingInfrastructure.Files.JsonFiles.PokemonFileInfo;
        // _outputHelper.WriteLine(thing.Length.ToString());
    }
    
    [Fact]
    public void Generate()
    {
        var inputCompilation = CSharpCompilation.Create("compilation",
            new[] { CSharpSyntaxTree.ParseText("public class R { }") },
            new[] { MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location) });
    
        var generator = new AdditionalFilesHelperGenerator();
    
        var generators = new ISourceGenerator[] { generator };
        var additionalFiles = new[]
        {
            new AnalyzerAdditionalText(@".\Assets\Teapot.obj", SourceText.From("Hello world")),
            new AnalyzerAdditionalText(@".\Assets\Models\Teapot.obj", SourceText.From("Hello world")),
            new AnalyzerAdditionalText(@".\Assets\Models\My Hole\State.obj", SourceText.From("Hello world")),
            new AnalyzerAdditionalText(@".\Assets\Models\Bob\MtL\Teapot.mtl", SourceText.From("Hello world")),
        };
    
        CSharpGeneratorDriver.Create(generators, optionsProvider: new TestOptionsProvider(), additionalTexts: additionalFiles).RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);
    
        _outputHelper.WriteLine(string.Join(Environment.NewLine, diagnostics.Select(x => x.GetMessage())));
        _outputHelper.WriteLine(string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Select(x => x.ToString())));
    }
}