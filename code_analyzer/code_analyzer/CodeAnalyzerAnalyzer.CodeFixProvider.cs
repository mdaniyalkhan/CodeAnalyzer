using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using code_analyzer.common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace code_analyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CodeAnalyzerCodeFixProvider)), Shared]
    public class CodeAnalyzerCodeFixProvider : CodeFixProvider
    {
        private const string PrivateModifier = "private";
        private const string Title = "Encapsulate Field";
        private const string FieldPrefix = "_";

        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(RuleId.EncapsulateFieldRuleId);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var node = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<FieldDeclarationSyntax>().First();

            if (!(node is FieldDeclarationSyntax field) ||
                field.Modifiers
                    .ToFullString()
                    .Contains(PrivateModifier))
            {
                return;
            }

            context.RegisterCodeFix(
                CodeAction.Create(
                    Title,
                    x => EncapsulatePublicProtectedField(context.Document, node, x),
                    Title),
                diagnostic);
        }

        private async Task<Document> EncapsulatePublicProtectedField(Document document, BaseFieldDeclarationSyntax fieldDeclaration, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var declaration = fieldDeclaration.Declaration;

            var propertyName = ConvertName(declaration.Variables[0].Identifier.ValueText);
            var propertyNode = PropertyDeclaration(declaration.Type, propertyName)
                .WithModifiers(fieldDeclaration.Modifiers)
                .WithAttributeLists(fieldDeclaration.AttributeLists)
                .AddAccessorListAccessors(AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)))
                .AddAccessorListAccessors(AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));

            var newRoot = root.ReplaceNode(fieldDeclaration, propertyNode);
            var newDocument = document.WithSyntaxRoot(newRoot);

            return newDocument;
        }

        private string ConvertName(string oldName)
        {
            oldName = oldName.Replace(FieldPrefix, string.Empty);
            return char.ToUpperInvariant(oldName[0]) + oldName.Substring(1);
        }
    }
}