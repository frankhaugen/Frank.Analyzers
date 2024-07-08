using Frank.Analyzers.Tests.TestingInfrastructure;
using Frank.Refactoring.AutoProperties;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Xunit.Abstractions;

namespace Frank.Analyzers.Tests.Refactorings;

public class AutoPropertyRefactroringTests : CodeRefactoringProviderTestFixture
{
    private readonly ITestOutputHelper _outputHelper;

    public AutoPropertyRefactroringTests(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }
    
    [Fact]
    public async Task TestRefactor()
    {
        // Arrange
        var code = """
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
                            
                            [||]public string Name
                            {
                                get => _name;
                                set => _name = value;
                            }
                        }
                    }
                    """;
        
        var fixedCode = """
                        using System;
                        using System.Collections.Generic;
                        using System.Linq;
                        using System.Text;
                        using System.Threading.Tasks;
                        
                        namespace Frank.Refactoring.AutoProperties
                        {
                            public class AutoPropertyRefactor
                            {
                                public string Name { get; set; }
                            }
                        }
                        """;

        Test(code, fixedCode, 0, true);
    }
    
    [Fact]
    public async Task TestRefactor2()
    {
        // Arrange
        var code = """
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
                            
                            [||]public string Name
                            {
                                get { return _name; }
                                set { _name = value; }
                            }
                        }
                    }
                    """;
        
        var fixedCode = """
                        using System;
                        using System.Collections.Generic;
                        using System.Linq;
                        using System.Text;
                        using System.Threading.Tasks;
                        
                        namespace Frank.Refactoring.AutoProperties
                        {
                            public class AutoPropertyRefactor
                            {
                                public string Name { get; set; }
                            }
                        }
                        """;

        Test(code, fixedCode, 0, true);
    }

    /// <inheritdoc />
    protected override string LanguageName { get; } = LanguageNames.CSharp;

    /// <inheritdoc />
    protected override CodeRefactoringProvider CreateCodeRefactoringProvider { get; } = new ConvertToAutoPropertyCodeRefactoringProvider();
}