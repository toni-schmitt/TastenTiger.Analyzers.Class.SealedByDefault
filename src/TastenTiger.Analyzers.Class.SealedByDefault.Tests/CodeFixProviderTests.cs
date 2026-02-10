using System.Threading.Tasks;
using Xunit;
using Verifier = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<
    TastenTiger.Analyzers.Class.SealedByDefault.SyntaxAnalyzer,
    TastenTiger.Analyzers.Class.SealedByDefault.CodeFix.CodeFixProvider>;

namespace TastenTiger.Analyzers.Class.SealedByDefault.Tests;

public class CodeFixProviderTests
{
    [Fact]
    public async Task ClassWithMyCompanyTitle_ReplaceWithCommonKeyword()
    {
        const string violatingCode = """

                                     public class NonSealedClass
                                     {
                                     }

                                     """;

        const string expectedFix = """

                                   public sealed class NonSealedClass
                                   {
                                   }

                                   """;

        var expected = Verifier.Diagnostic()
            .WithLocation(2, 14)
            .WithArguments("NonSealedClass");
        await Verifier.VerifyCodeFixAsync(violatingCode, expected, expectedFix);
    }
}