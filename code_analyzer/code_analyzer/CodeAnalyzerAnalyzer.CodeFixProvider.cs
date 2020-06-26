using System.Collections.Generic;
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
using Microsoft.CodeAnalysis.Editing;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace code_analyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CodeAnalyzerCodeFixProvider)), Shared]
    public class CodeAnalyzerCodeFixProvider : CodeFixProvider
    {
        private const string PrivateModifier = "private";
        private const string Title = "Encapsulate Field";
        private const string TitleMagicValues = "Replace Magic Values";
        private const string FieldPrefix = "_";

        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
            RuleId.EncapsulateFieldRuleId,
            RuleId.ReplaceMagicValues);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var node = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<FieldDeclarationSyntax>().FirstOrDefault();
            var classNode = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().FirstOrDefault();

            if (node != null &&
                !node.Modifiers
                    .ToFullString()
                    .Contains(PrivateModifier))
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        Title,
                        x => EncapsulatePublicProtectedField(context.Document, node, x),
                        Title),
                    diagnostic);
            }

            if (classNode != null)
            {
                context.RegisterCodeFix(CodeAction.Create(
                    TitleMagicValues,
                    x => ReplaceMagicValues(context.Document, classNode, x),
                    TitleMagicValues),
                    diagnostic);
            }
        }

        private static async Task<Document> ReplaceMagicValues(Document document, SyntaxNode node, CancellationToken cancellationToken)
        {
            var constantMapping = new List<ConstantMapper>();
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken);

            var literalsInsideMethods = node
                .DescendantNodes<LiteralExpressionSyntax>()
                .Where(x => x.Ancestors<MethodDeclarationSyntax>().Any()).ToList();

            var constants = node.DescendantNodes<FieldDeclarationSyntax>().Where(x => x.Modifiers.ToString().Contains("const"));

            foreach (var constant in constants)
            {
                foreach (var variable in constant.Declaration.Variables)
                {
                    var matched = literalsInsideMethods.Where(x => x.ToString() == variable.Initializer.Value.ToString());
                    constantMapping.AddRange(
                        matched
                            .Select(literal => new ConstantMapper
                            {
                                ConstantName = variable.Identifier.ValueText,
                                Literal = literal
                            }));
                }
            }

            foreach (var constantMapper in constantMapping)
            {
                editor.ReplaceNode(constantMapper.Literal, IdentifierName(constantMapper.ConstantName));
            }

            return editor.GetChangedDocument();
        }

        private static async Task<Document> EncapsulatePublicProtectedField(Document document, BaseFieldDeclarationSyntax fieldDeclaration, CancellationToken cancellationToken)
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

        private static string ConvertName(string oldName)
        {
            oldName = oldName.Replace(FieldPrefix, string.Empty);
            return char.ToUpperInvariant(oldName[0]) + oldName.Substring(1);
        }
    }
}