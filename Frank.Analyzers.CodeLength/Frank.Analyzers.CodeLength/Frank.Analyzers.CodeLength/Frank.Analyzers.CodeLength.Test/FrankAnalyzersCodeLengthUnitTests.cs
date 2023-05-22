using Microsoft.CodeAnalysis.CSharp.Testing.XUnit;
using Microsoft.CodeAnalysis.Testing;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Frank.Analyzers.CodeLength.Test
{
	public class FrankAnalyzersCodeLengthUnitTest
	{
		[Fact]
		public async Task TestMethod1()
		{
			var testCode = await File.ReadAllTextAsync("C:\\repos\\frankhaugen\\Frank.Libraries\\src\\Frank.Libraries.Calculators\\FluentCalculation\\FluentCalculatorPrimitivesConversions.cs");

			var result = new DiagnosticResult(FrankAnalyzersCodeLengthAnalyzer.Rule).WithArguments("Test0", FrankAnalyzersCodeLengthAnalyzer.MaxCodeLine);
			await AnalyzerVerifier<FrankAnalyzersCodeLengthAnalyzer>.VerifyAnalyzerAsync(testCode, result);
		}
	}
}