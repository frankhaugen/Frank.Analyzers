using ClangSharp;
using ClangSharp.Interop;
using Microsoft.CodeAnalysis;
using Diagnostic = Microsoft.CodeAnalysis.Diagnostic;

namespace Frank.SourceGenerators.CppInteropts
{
    [Generator(LanguageNames.CSharp)]
    public class CppInteropGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required
            Console.WriteLine("CppInteropGenerator initialized");
        }

        public void Execute(GeneratorExecutionContext context)
        {
            // Find the C++ files in the assets folder
            var cppFiles = context.AdditionalFiles
                .Where(file => Path.GetExtension(file.Path) == ".cpp" || Path.GetExtension(file.Path) == ".h" || Path.GetExtension(file.Path) == ".c")
                .Where(file => file.Path.Contains("assets"))
                .Select(file => file.Path);

            foreach (var cppFile in cppFiles)
            {
                GeneratePInvokeCode(cppFile, context);
            }
        }

        private void GeneratePInvokeCode(string filePath, GeneratorExecutionContext context)
        {
            var outputBuilder = new PInvokeGeneratorConfiguration("c", "c17", "Frank.SourceGenerators.CppInteropts.Generated", "Bullet3Interop", headerFile: filePath, PInvokeGeneratorOutputMode.CSharp, PInvokeGeneratorConfigurationOptions.GenerateLatestCode);

            var index = CXIndex.Create();
            using var pinvokeGenerator = new PInvokeGenerator(outputBuilder);

            var translationUnit = CXTranslationUnit.Parse(index, filePath, new string[] { }, new CXUnsavedFile[] { }, CXTranslationUnit_Flags.CXTranslationUnit_None);

            if (translationUnit.NumDiagnostics > 0)
            {
                for (uint i = 0; i < translationUnit.NumDiagnostics; ++i)
                {
                    var diagnostic = translationUnit.GetDiagnostic(i);
                    Console.WriteLine(diagnostic.Format(CXDiagnosticDisplayOptions.CXDiagnostic_DisplayColumn | CXDiagnosticDisplayOptions.CXDiagnostic_DisplaySourceLocation));
                }
            }
            
            TranslationUnit tu = TranslationUnit.GetOrCreate(translationUnit);

            pinvokeGenerator.GenerateBindings(tu, filePath, new string[] {}, CXTranslationUnit_Flags.CXTranslationUnit_None);

            var diagnostics = pinvokeGenerator.Diagnostics;
            
            foreach (var diagnostic in diagnostics)
            {
                Console.WriteLine(diagnostic.ToString());
            }
            
            // context.AddSource(Path.GetFileNameWithoutExtension(filePath) + ".g.cs", SourceText.From(generatedCode, Encoding.UTF8));
        }
    }
}

