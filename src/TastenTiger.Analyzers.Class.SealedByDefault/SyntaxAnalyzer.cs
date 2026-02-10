using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace TastenTiger.Analyzers.Class.SealedByDefault;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class SyntaxAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "TA0002";

    private static readonly DiagnosticDescriptor Rule = new(DiagnosticId,
        "Class should be sealed by default",
        "Class should be sealed since no other type inherits {0}", "Usage",
        DiagnosticSeverity.Info, true, "All non-inherited classes must be sealed.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSymbolAction(AnalyzeSyntax, SymbolKind.NamedType);
    }

    private static void AnalyzeSyntax(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol namedTypeSymbol)
            return;

        if (namedTypeSymbol.TypeKind is not TypeKind.Class)
            return;

        if (namedTypeSymbol.IsAbstract || namedTypeSymbol.IsStatic || namedTypeSymbol.IsSealed)
            return;

        var hasDerivedType = context.Compilation.GetSymbolsWithName(_ => true, SymbolFilter.Type).Any(x =>
            (x as INamedTypeSymbol)?.BaseType?.Equals(namedTypeSymbol, SymbolEqualityComparer.Default) ?? false);

        if (hasDerivedType)
            return;

        context.ReportDiagnostic(Diagnostic.Create(Rule, namedTypeSymbol.Locations[0], namedTypeSymbol.Name));
    }
}