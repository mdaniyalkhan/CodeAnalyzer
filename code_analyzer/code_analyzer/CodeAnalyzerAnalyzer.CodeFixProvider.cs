using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Text.RegularExpressions;
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
        private const string EncapsulateFieldTitle = "Encapsulate Field";
        private const string TitleMagicValues = "Replace Magic Values";
        private const string TitleUseLambdaExpression = "Use Lambda Expressions";
        private const string SimplifyFakesTitle = "Simplify Fakes";
        private const string RemoveFakesTitle = "Remove Fakes";
        private const string FieldPrefix = "_";
        private const string String = "string";
        private const string Const = "const";
        private const string RemoveUnnecessaryShimsContextTitle = "Remove Unnecessary Shims Context";

        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
            RuleId.EncapsulateFieldRuleId,
            RuleId.ReplaceMagicValues,
            RuleId.UnnecessaryShimsContext,
            RuleId.SimplifyFakes,
            RuleId.RemoveFakes);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var node = root.FindToken(diagnosticSpan.Start)
                .Parent
                .AncestorsAndSelf().OfType<FieldDeclarationSyntax>()
                .FirstOrDefault();

            var classNode = root.FindToken(diagnosticSpan.Start)
                .Parent
                .AncestorsAndSelf()
                .OfType<ClassDeclarationSyntax>()
                .FirstOrDefault();

            var member = root.FindToken(diagnosticSpan.Start)
                .Parent
                .AncestorsAndSelf()
                .OfType<MemberAccessExpressionSyntax>()
                .FirstOrDefault();

            if (node != null &&
                !node.Modifiers
                    .ToFullString()
                    .Contains(PrivateModifier))
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        EncapsulateFieldTitle,
                        x => EncapsulatePublicProtectedField(context.Document, node, x),
                        EncapsulateFieldTitle),
                    diagnostic);
            }

            if(member != null)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        SimplifyFakesTitle,
                        x => SimplifyFakes(context.Document, member, x),
                        SimplifyFakesTitle),
                    diagnostic);

                context.RegisterCodeFix(
                    CodeAction.Create(
                        RemoveFakesTitle,
                        x => RemoveFakes(context.Document, member, x),
                        RemoveFakesTitle),
                    diagnostic);
            }

            if (classNode != null)
            {
                context.RegisterCodeFix(CodeAction.Create(
                        TitleMagicValues,
                        x => ReplaceMagicValues(context.Document, classNode, x),
                        TitleMagicValues),
                    diagnostic);

                context.RegisterCodeFix(CodeAction.Create(
                        RemoveUnnecessaryShimsContextTitle,
                        x => RemoveUnnecessaryShimsContext(context.Document, classNode, x),
                        RemoveUnnecessaryShimsContextTitle),
                    diagnostic);

                context.RegisterCodeFix(CodeAction.Create(
                        TitleUseLambdaExpression,
                        x => UseLambdaExpression(context.Document, classNode, x),
                        TitleUseLambdaExpression),
                    diagnostic);
            }
        }

        private async Task<Document> SimplifyFakes(Document document, MemberAccessExpressionSyntax node, CancellationToken cancellationToken)
        {
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken);

            var claz = node
              .Ancestors<ClassDeclarationSyntax>()
              .FirstOrDefault();

            var fakes = claz
                .DescendantNodes<MemberAccessExpressionSyntax>()
                .Where(x => x.Expression.ToString().Trim().Equals(node.Expression.ToString().Trim()))
                .ToList();

            var clazName = node.Expression.ToString().Split('.')[0];
            var name = IdentifierName(clazName);
            var left = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, name, IdentifierName("Constructor"));

            var expressions = new List<ExpressionSyntax>();
            var strExpressions = new List<string>();
            foreach (var fake in fakes)
            {
                if (fake.Parent is MemberAccessExpressionSyntax member && member.Parent is AssignmentExpressionSyntax parent &&
                    !strExpressions.Any(x => x.Equals(parent.Right.ToString())))
                {
                    if (parent.Right is SimpleLambdaExpressionSyntax lambda)
                    {
                        var assignmentExpression = AssignmentExpression(
                            SyntaxKind.SimpleAssignmentExpression,
                            IdentifierName(member.Name.Identifier.ValueText),
                            ParenthesizedLambdaExpression(lambda.Body));
                        expressions.Add(assignmentExpression);
                        strExpressions.Add(lambda.ToString());
                    }

                    if (parent.Right is ParenthesizedLambdaExpressionSyntax parenthesized)
                    {
                        var parameters = new List<ParameterSyntax>();
                        for (var index = 1; index < parenthesized.ParameterList.Parameters.Count; index++)
                        {
                            parameters.Add(parenthesized.ParameterList.Parameters[index]);
                        }

                        var assignmentExpression = AssignmentExpression(
                            SyntaxKind.SimpleAssignmentExpression,
                            IdentifierName(member.Name.Identifier.ValueText),
                            ParenthesizedLambdaExpression(ParameterList()
                                    .AddParameters(parameters.ToArray()), parenthesized.Body));
                        expressions.Add(assignmentExpression);
                        strExpressions.Add(parenthesized.ToString());
                    }
                }
            }

            var initializer = InitializerExpression(SyntaxKind.ObjectInitializerExpression)
                .AddExpressions(expressions.ToArray());
            var shimObject = ObjectCreationExpression(ParseTypeName(clazName))
                .WithInitializer(initializer)
                .AddArgumentListArguments(Argument(IdentifierName("_")));
            var local = LocalDeclarationStatement(VariableDeclaration(ParseTypeName("var")))
                .AddDeclarationVariables(
                    VariableDeclarator("instance")
                        .WithInitializer(EqualsValueClause(shimObject)));
            var right = SimpleLambdaExpression(Parameter(ParseToken("_")), Block(
                local,
                ExpressionStatement(
                    InvocationExpression(
                        MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName("instance"), IdentifierName("BehaveAsDefaultValue"))))));
            var finalNode = ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, left, right).NormalizeWhitespace(), Token(SyntaxKind.SemicolonToken));

            editor.ReplaceNode(node.Ancestors<ExpressionStatementSyntax>().First(), finalNode);
            return editor.GetChangedDocument();
        }

        private async Task<Document> RemoveFakes(Document document, MemberAccessExpressionSyntax node, CancellationToken cancellationToken)
        {
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken);

            var claz = node
              .Ancestors<ClassDeclarationSyntax>()
              .FirstOrDefault();

            var fakes = claz
                .DescendantNodes<MemberAccessExpressionSyntax>()
                .Where(x => x.Expression.ToString().Trim().Equals(node.Expression.ToString().Trim()))
                .ToList();

            foreach (var fake in fakes)
            {
                if (fake.Parent.Parent is AssignmentExpressionSyntax parent)
                {
                    if (parent.Right is SimpleLambdaExpressionSyntax || 
                        parent.Right is ParenthesizedLambdaExpressionSyntax)
                    {
                        editor.RemoveNode(fake.Ancestors<ExpressionStatementSyntax>().First());
                    }
                }
            }

            return editor.GetChangedDocument();
        }

        private static async Task<Document> UseLambdaExpression(Document document, ClassDeclarationSyntax node, CancellationToken cancellationToken)
        {
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken);
            var nodes = node.DescendantNodes<ReturnStatementSyntax>()
                .Where(x => x.Parent is BlockSyntax &&
                            x.Parent.ChildNodes().Count() == 1 &&
                            (x.Parent.Parent is SimpleLambdaExpressionSyntax ||
                             x.Parent.Parent is ParenthesizedLambdaExpressionSyntax)).ToList();

            foreach (var nd in nodes)
            {
                if (nd.Parent.Parent is SimpleLambdaExpressionSyntax lambdaExpression)
                {
                    editor.ReplaceNode(lambdaExpression, SimpleLambdaExpression(lambdaExpression.Parameter, nd.Expression));
                }

                if (nd.Parent.Parent is ParenthesizedLambdaExpressionSyntax paramLambdaExpression)
                {
                    editor.ReplaceNode(paramLambdaExpression, ParenthesizedLambdaExpression(nd.Expression));
                }
            }

            return editor.GetChangedDocument();
        }

        private static async Task<Document> RemoveUnnecessaryShimsContext(Document document, SyntaxNode node, CancellationToken cancellationToken)
        {
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken);
            var shimsContexts = node
                .DescendantNodes<BlockSyntax>()
                .Where(x => x.Parent is UsingStatementSyntax parent &&
                            parent.Expression.ToString().Contains("ShimsContext.Create()") &&
                            !parent.Statement.ToString().Contains("Shim"));

            foreach (var context in shimsContexts)
            {
                foreach (var statement in context.Statements)
                {
                    editor.InsertBefore(context.Parent, statement);
                }

                editor.RemoveNode(context.Parent);
            }

            return editor.GetChangedDocument();
        }


        private static async Task<Document> ReplaceMagicValues(Document document, SyntaxNode node, CancellationToken cancellationToken)
        {
            var constantMapping = new List<ConstantMapper>();
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken);

            var literalsInsideMethods = node
                .DescendantNodes<LiteralExpressionSyntax>()
                .Where(x => x.Ancestors<MethodDeclarationSyntax>().Any()).ToList();

            var constants = node
                .DescendantNodes<FieldDeclarationSyntax>()
                .Where(x => x.Modifiers.ToString().Contains(Const)).ToList();

            foreach (var constant in constants)
            {
                foreach (var variable in constant.Declaration.Variables)
                {
                    var matched = literalsInsideMethods
                        .Where(x => x.ToString() == variable.Initializer.Value.ToString()).ToList();

                    constantMapping.AddRange(
                        matched
                            .Select(literal => new ConstantMapper
                            {
                                ConstantName = variable.Identifier.ValueText,
                                Literal = literal
                            }));

                    literalsInsideMethods = literalsInsideMethods.Except(matched).ToList();
                }
            }

            foreach (var constantMapper in constantMapping)
            {
                editor.ReplaceNode(constantMapper.Literal, IdentifierName(constantMapper.ConstantName));
            }

            if (constants.Any())
            {
                var pendingOnes = literalsInsideMethods
                    .Where(x => x.Kind() == SyntaxKind.StringLiteralExpression)
                    .GroupBy(grp => grp.ToString()).Select(x => new
                    {
                        Expression = x.Key,
                        Literals = x
                    });

                foreach (var pending in pendingOnes)
                {
                    var constantName = pending.Expression.FixMemberName().Capitalize();
                    if (Regex.IsMatch(constantName, "^[a-zA-Z_][a-zA-Z_0-9]*$") &&
                        constantMapping.All(x => x.ConstantName != constantName))
                    {
                        var lastConstant = constants.Last();
                        var token = Literal(pending.Expression.Trim('\"'));
                        editor.InsertAfter(
                            lastConstant,
                            newNode: FieldDeclaration(
                                    lastConstant.AttributeLists,
                                    lastConstant.Modifiers,
                                    VariableDeclaration(ParseTypeName(String)))
                                .AddDeclarationVariables(VariableDeclarator($" {constantName}")
                                    .WithInitializer(
                                        EqualsValueClause(LiteralExpression(SyntaxKind.StringLiteralExpression,
                                            token))))
                                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));

                        foreach (var literal in pending.Literals)
                        {
                            editor.ReplaceNode(literal, IdentifierName(constantName));
                        }
                    }
                }
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
            var name = oldName.Replace(FieldPrefix, string.Empty);
            return char.ToUpperInvariant(name[0]) + name.Substring(1);
        }
    }
}
