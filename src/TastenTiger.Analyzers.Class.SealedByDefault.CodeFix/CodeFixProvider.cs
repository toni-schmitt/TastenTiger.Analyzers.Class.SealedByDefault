using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace TastenTiger.Analyzers.Class.SealedByDefault.CodeFix;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CodeFixProvider))]
[Shared]
public class CodeFixProvider : Microsoft.CodeAnalysis.CodeFixes.CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } =
        [SyntaxAnalyzer.DiagnosticId];

    public override FixAllProvider? GetFixAllProvider()
    {
        return WellKnownFixAllProviders.BatchFixer;
    }

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics.Single();

        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        var syntaxNode = root?.FindNode(diagnostic.Location.SourceSpan);

        if (syntaxNode is not (RecordDeclarationSyntax or ClassDeclarationSyntax))
            return;

        context.RegisterCodeFix(
            CodeAction.Create("Make class sealed",
                ct => AddSealedModifier(context.Document, (syntaxNode as TypeDeclarationSyntax)!, ct),
                nameof(CodeFixProvider)),
            diagnostic);
    }

    private static async Task<Document> AddSealedModifier(Document document, TypeDeclarationSyntax typeDecl,
        CancellationToken ct)
    {
        var editor = await DocumentEditor.CreateAsync(document, ct).ConfigureAwait(false);

        var newDecl = typeDecl.AddModifiers(SyntaxFactory.Token(SyntaxKind.SealedKeyword));

        editor.ReplaceNode(typeDecl, newDecl);

        return editor.GetChangedDocument();
    }
}