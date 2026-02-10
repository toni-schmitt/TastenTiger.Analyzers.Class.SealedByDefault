using System.Threading.Tasks;
using Xunit;
using Verifier = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<
    TastenTiger.Analyzers.Class.SealedByDefault.SyntaxAnalyzer>;

namespace TastenTiger.Analyzers.Class.SealedByDefault.Tests;

public class SyntaxAnalyzerTests
{
    [Fact]
    public async Task NonSealed_NonInherited_Class_AlertsDiagnostic()
    {
        const string violatingCode = """

                                     public class NonSealedClass
                                     {
                                     }

                                     """;

        var expected = Verifier.Diagnostic()
            .WithLocation(2, 14)
            .WithArguments("NonSealedClass");
        await Verifier.VerifyAnalyzerAsync(violatingCode, expected).ConfigureAwait(false);
    }

    [Fact]
    public async Task InheritorChain_LastNonSealedClass_AlertsDiagnostic()
    {
        const string violatingCode = """

                                     public abstract class BaseClass {}
                                     public class Inheritor1 : BaseClass {}
                                     public class Inheritor2 : BaseClass {}
                                     public class Inheritor3: Inheritor1 {}
                                     public class Inheritor4: Inheritor2 {}
                                     public sealed class SealedEndInheritor : Inheritor3 {}
                                     public class NonSealedEndInheritor : Inheritor4 {}

                                     """;

        var expected = Verifier.Diagnostic()
            .WithLocation(8, 14)
            .WithArguments("NonSealedEndInheritor");
        await Verifier.VerifyAnalyzerAsync(violatingCode, expected).ConfigureAwait(false);
    }

    [Fact]
    public async Task NonInheritableClass_AlertsNothing()
    {
        const string complyingCode = """

                                     public abstract class AbstractClass {}
                                     public static class StaticClass {}

                                     """;

        await Verifier.VerifyAnalyzerAsync(complyingCode).ConfigureAwait(false);
    }

    [Fact]
    public async Task NonSealed_NonStandardClasses_AlertDiagnostic()
    {
        const string violatingCode = """

                                     public abstract record class AbstractRecordClass {}
                                     public record class NonSealedRecordClass {}
                                     public sealed record class SealedRecordClass {}

                                     """;

        var expected = Verifier.Diagnostic()
            .WithLocation(3, 21)
            .WithArguments("NonSealedRecordClass");
        await Verifier.VerifyAnalyzerAsync(violatingCode, expected).ConfigureAwait(false);
    }
}