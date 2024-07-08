using Frank.Analyzers.AutoProperties;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace Frank.Analyzers.Tests.Refactorings;

public class ConvertToAutoPropertyCodeRefactoringProviderTests : CSharpCodeRefactoringTest<ConvertToAutoPropertyCodeRefactoringProvider, DefaultVerifier>
{
    private readonly ITestOutputHelper _outputHelper;

    public ConvertToAutoPropertyCodeRefactoringProviderTests(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task Test()
    {
        TestCode = """
                   public class Test
                   {
                       private int _field;
                       [||]public int Property
                       {
                           get => _field;
                           set => _field = value;
                       }
                   }
                   """.ReplaceLineEndings();

        FixedCode = """
                       public class Test
                       {
                           public int Property { get; set; }
                       }
                       """.ReplaceLineEndings();
        
        await RunAsync();
    }
}