using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using code_analyzer.common;

namespace code_analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public partial class CodeAnalyzerAnalyzer : DiagnosticAnalyzer
    {
        private const string Category = "Severes";
        private const string PublicModifier = "public";
        private const string ProtectedModifier = "protected";
        private const string ConstantModifier = "const";

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
            EncapsulateFieldRule,
            AggregateExceptionRule,
            BlankCodeRule,
            ContextualKeyWordRule,
            DuplicateShims,
            EnumDefaultValueRule,
            ExceptionWithoutContextRule,
            InappropriateUsageOfPropertyRule,
            LiskovSubsitutionPrincipleRule,
            MethodWithBoolAsParameterRule,
            MethodWithMoreThanSevenParametersRule,
            MissingParameterNullValidationRule,
            MissingConstructorParameterNullValidationRule,
            NonPrivateConstantsRule,
            PreferClassOverStructRule,
            ReplaceMagicValues,
            UseLambdaExpression,
            ShouldlySingleAssertInUowRule,
            SimplifyShims,
            SwitchWithoutDefaultCaseRule,
            TestCaseArgumentsRule,
            ToArrayToListInsideForeachDeclaration,
            UnnecessaryShimsContext,
            ParameterNotReassignedRule,
            ParameterUnusedRule,
            SimplifyFakes,
            RemoveFakes,
            SimplifyFakesObject);

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
            {
                return;
            }
            context.RegisterSyntaxNodeAction(AnalyzeFieldToBeEncapsulate, SyntaxKind.FieldDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeMagicValues, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeUseLambdaExpressions, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeContextualKeyWord, SyntaxKind.IdentifierName, SyntaxKind.Parameter);
            context.RegisterSyntaxNodeAction(AnalyzeClassesAreNounRule, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeBlankBlockCode, SyntaxKind.Block);
            context.RegisterSyntaxNodeAction(
                AnalyzeLiskovSubstitutionPrincipal,
                SyntaxKind.Parameter,
                SyntaxKind.GenericName,
                SyntaxKind.PredefinedType);

            context.RegisterSyntaxNodeAction(AnalyzeToArrayToListInForeachDeclaration, SyntaxKind.ForEachStatement);
            context.RegisterSyntaxNodeAction(AnalyzeSwitchWithoutDefaultLabel, SyntaxKind.SwitchStatement);
            context.RegisterSyntaxNodeAction(AnalyzeGenericExceptionCode, SyntaxKind.IdentifierName);
            context.RegisterSyntaxNodeAction(AnalyzeExceptionWithoutContextCode, SyntaxKind.ObjectCreationExpression);
            context.RegisterSyntaxNodeAction(AnalyzeEnumWithoutDefaultCode, SyntaxKind.EnumDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeMethodWithMoreThanSevenParameters, SyntaxKind.ParameterList);
            context.RegisterSyntaxNodeAction(AnalyzeMethodWithBoolAsParameter, SyntaxKind.ParameterList);
            context.RegisterSyntaxNodeAction(AnalyzeStructCode, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeInappropriateUsageOfProperty, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeShouldlySingleAssertInUow, SyntaxKind.IdentifierName);
            context.RegisterSyntaxNodeAction(AnalyzeNonPrivateConstants, SyntaxKind.FieldDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeTestCaseArguments, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeTestCaseDataArguments, SyntaxKind.ArrayInitializerExpression);
            context.RegisterSyntaxNodeAction(AnalyzeTestCaseDataArguments, SyntaxKind.CollectionInitializerExpression);
            context.RegisterSyntaxNodeAction(AnalyzeUnnecessaryShimsContext, SyntaxKind.UsingStatement);
            context.RegisterSyntaxNodeAction(AnalyzeShimsMisuse, SyntaxKind.SimpleMemberAccessExpression);
            context.RegisterSyntaxNodeAction(AnalyzeDuplicateShims, SyntaxKind.SimpleMemberAccessExpression);
            context.RegisterSyntaxNodeAction(AnalyzeMissingParameterNullValidation, SyntaxKind.Parameter);
            context.RegisterSyntaxNodeAction(AnalyzeMissingConstructorParameterNullValidation, SyntaxKind.Parameter);
            context.RegisterSyntaxNodeAction(AnalyzeParameterNotReassigned, SyntaxKind.Parameter);
            context.RegisterSyntaxNodeAction(AnalyzeParameterUnused, SyntaxKind.Parameter);
            context.RegisterSyntaxNodeAction(AnalyzeSimpleFakes, SyntaxKind.SimpleMemberAccessExpression);
            context.RegisterSyntaxNodeAction(AnalyzeRemoveFakes, SyntaxKind.SimpleMemberAccessExpression);
            context.RegisterSyntaxNodeAction(AnalyzeSimplifyFakesObject, SyntaxKind.ObjectCreationExpression);
        }

        private void AnalyzeSimplifyFakesObject(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is ObjectCreationExpressionSyntax node))
            {
                return;
            }

            if (node.Type.ToString().StartsWith("Shim"))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    SimplifyFakesObject, node.GetLocation(),
                    "Simplify Fakes Object"));
            }
        }

        private static void AnalyzeRemoveFakes(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is MemberAccessExpressionSyntax node))
            {
                return;
            }

            if (node.Parent is AssignmentExpressionSyntax parent && parent.Parent is ExpressionStatementSyntax)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    SimplifyFakes, node.GetLocation(),
                    "Remove Fakes"));
            }
        }

        private static void AnalyzeSimpleFakes(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is MemberAccessExpressionSyntax node))
            {
                return;
            }

            if (node.Parent is AssignmentExpressionSyntax parent && parent.Parent is ExpressionStatementSyntax)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    SimplifyFakes, node.GetLocation(),
                    "Simplify Fakes"));
            }
        }

        private static void AnalyzeDuplicateShims(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is MemberAccessExpressionSyntax node))
            {
                return;
            }

            if (!node.ToString().Contains(".AllInstances."))
            {
                return;
            }

            var method = node.Ancestors<MethodDeclarationSyntax>().FirstOrDefault();
            if (method?
                    .DescendantNodes<MemberAccessExpressionSyntax>()
                    .Count(x => x.ToString() == node.ToString()) > 1)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    DuplicateShims, node.GetLocation(),
                    "Remove Duplicate Shims"));
            }
        }

        private static void AnalyzeShimsMisuse(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is MemberAccessExpressionSyntax node))
            {
                return;
            }

            if (!node.ToString().EndsWith("AllInstances"))
            {
                return;
            }

            var method = node.Ancestors<MethodDeclarationSyntax>().FirstOrDefault();
            if (method?
                    .DescendantNodes<ObjectCreationExpressionSyntax>()
                    .Any(obj => node.ToString() == $"{obj.Type}.AllInstances") == true)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    SimplifyShims, node.GetLocation(),
                    "Simplify Shims using existing object instances"));
            }
        }

        private static void AnalyzeUnnecessaryShimsContext(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is UsingStatementSyntax node))
            {
                return;
            }

            if (!node.Expression?.ToString().Contains("ShimsContext.Create()") == true)
            {
                return;
            }

            if (!node.Declaration?.ToString().Contains("ShimsContext.Create()") == true)
            {
                return;
            }

            if (node.Statement.ToString().Contains("Shim"))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(
                UnnecessaryShimsContext, node.GetLocation(), "Remove Unnecessary Shims Context"));
        }

        private static void AnalyzeMagicValues(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node;

            if (!(node is ClassDeclarationSyntax))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(
                ReplaceMagicValues, node.GetLocation(),
                "Replace Magic Values inside methods with respect to defined constants"));
        }

        private static void AnalyzeUseLambdaExpressions(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node;

            if (!(node is ClassDeclarationSyntax))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(
                UseLambdaExpression, node.GetLocation(),
                "Simplify Code Using Lambda Expressions"));
        }

        private static void AnalyzeTestCaseDataArguments(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node;
            if (!(node is InitializerExpressionSyntax initializer))
            {
                return;
            }

            if (!initializer.ChildNodes().All(y => y.Kind() == SyntaxKind.ObjectCreationExpression &&
                                                   (y as ObjectCreationExpressionSyntax)?.Type?.ToString() ==
                                                   "TestCaseData"))
            {
                return;
            }

            var testCases = initializer.ChildNodes().Cast<ObjectCreationExpressionSyntax>().ToList();
            if (!testCases.Any())
            {
                return;
            }

            var message = string.Empty;
            if (testCases.Count == 1)
            {
                message = "Simplify Test Code by removing single Test Case";
            }
            else
            {
                var firstAttribute = testCases.First();
                if (firstAttribute.ArgumentList != null)
                    for (var attrArgIndex = 0;
                        attrArgIndex < firstAttribute.ArgumentList.Arguments.Count;
                        attrArgIndex++)
                    {
                        if (testCases.Skip(1).All(x => x.ArgumentList != null && x.ArgumentList.Arguments[attrArgIndex].ToString() ==
                                                       firstAttribute.ArgumentList.Arguments[attrArgIndex].ToString()))
                        {
                            message = "Simplify Test Code by removing same arguments across all test cases";
                            break;
                        }
                    }
            }

            if (!string.IsNullOrWhiteSpace(message))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    TestCaseArgumentsRule, node.GetLocation(), message));
            }
        }

        private static void AnalyzeTestCaseArguments(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node;
            if (!(node is MethodDeclarationSyntax method))
            {
                return;
            }

            var attributes = method.AttributeLists.Where(x => x.Attributes.Any(y => y.Name.ToString() == "TestCase"))
                .SelectMany(x => x.Attributes).ToList();

            if (!attributes.Any())
            {
                return;
            }

            var message = string.Empty;
            if (attributes.Count == 1)
            {
                message = "Simplify Test Code by removing single Test Case";
            }
            else
            {
                var firstAttribute = attributes.First();
                for (var attrArgIndex = 0; attrArgIndex < firstAttribute.ArgumentList.Arguments.Count; attrArgIndex++)
                {
                    if (attributes.Skip(1).All(x => x.ArgumentList.Arguments[attrArgIndex].ToString() ==
                                                    firstAttribute.ArgumentList.Arguments[attrArgIndex].ToString()))
                    {
                        message = "Simplify Test Code by removing same arguments across all test cases";
                        break;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(message))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    TestCaseArgumentsRule, attributes.First().GetLocation(), message));
            }
        }

        private static void AnalyzeNonPrivateConstants(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node;
            if (!(node is FieldDeclarationSyntax field))
            {
                return;
            }

            var privateField = !(field.Modifiers.ToString().Contains(PublicModifier) ||
                                 field.Modifiers.ToString().Contains(ProtectedModifier));
            if (privateField)
            {
                return;
            }

            var containConstant = field.Modifiers.ToString().Contains(ConstantModifier);
            if (!containConstant)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(
                NonPrivateConstantsRule, node.GetLocation(),
                "Avoid protected / public constants for values that might change"));
        }

        private static void AnalyzeShouldlySingleAssertInUow(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node;
            if (!(node is IdentifierNameSyntax syntax) ||
                !syntax.Identifier.ValueText.Equals("ShouldSatisfyAllConditions", StringComparison.CurrentCulture))
            {
                return;
            }

            var invocation = node.Ancestors<InvocationExpressionSyntax>().FirstOrDefault();
            if (invocation == null || invocation.ArgumentList.Arguments.Count > 1)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(
                ShouldlySingleAssertInUowRule, node.GetLocation(),
                "Shouldly - Avoid Assert Single Item With UnitOfWork"));
        }

        private static void AnalyzeInappropriateUsageOfProperty(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node;
            if (!(node is PropertyDeclarationSyntax property))
            {
                return;
            }

            if (property.DescendantNodes<ArrowExpressionClauseSyntax>().Count < 4)
            {
                return;
            }

            if (!(property.DescendantNodes<InvocationExpressionSyntax>().Any() ||
                  property.DescendantNodes<ObjectCreationExpressionSyntax>().Any()))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(
                InappropriateUsageOfPropertyRule, node.GetLocation(),
                "This code declares a property that should be a method."));
        }

        private static void AnalyzeStructCode(SyntaxNodeAnalysisContext context)
        {
            var root = context.Node as StructDeclarationSyntax;

            if (root == null)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(PreferClassOverStructRule, root.GetLocation(),
                "This code defines a `struct` that should be a `class`"));
        }

        private static void AnalyzeMethodWithBoolAsParameter(SyntaxNodeAnalysisContext context)
        {
            var root = context.Node as ParameterListSyntax;

            if (root == null ||
                root.Parent is ParenthesizedLambdaExpressionSyntax ||
                !root.Parameters.Any() ||
                root.Parameters.All(x =>
                {
                    var type = x.Type.ToString();
                    return type != "bool" && type != "Boolean";
                }))
            {
                return;
            }

            var method = root.Ancestors<MethodDeclarationSyntax>().FirstOrDefault();
            if (method != null && method.AttributeLists.Any())
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(
                MethodWithBoolAsParameterRule,
                root.GetLocation(),
                "This method receives a bool argument. This is prone to be against SRP from SOLID"));
        }

        private static void AnalyzeMethodWithMoreThanSevenParameters(SyntaxNodeAnalysisContext context)
        {
            var root = context.Node as ParameterListSyntax;

            if (root == null ||
                root.Parent is ParenthesizedLambdaExpressionSyntax ||
                root.Parameters.Count <= 7)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(MethodWithMoreThanSevenParametersRule, root.GetLocation(),
                "This method receives too many parameters (> 7)"));
        }

        private static void AnalyzeEnumWithoutDefaultCode(SyntaxNodeAnalysisContext context)
        {
            var root = context.Node as EnumDeclarationSyntax;

            if (root == null ||
                root.Members.Any(x => x.EqualsValue.Value.ToString() == "0"))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(EnumDefaultValueRule, root.GetLocation(),
                "This enumeration does not contain a value for 0 (zero)."));
        }

        private static void AnalyzeExceptionWithoutContextCode(SyntaxNodeAnalysisContext context)
        {
            var root = context.Node as ObjectCreationExpressionSyntax;

            if (root == null)
            {
                return;
            }

            var exceptionWithoutContext = root.ArgumentList != null &&
                                          root.Parent is ThrowStatementSyntax &&
                                          !root.ArgumentList.Arguments.Any();

            if (!exceptionWithoutContext)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(
                ExceptionWithoutContextRule,
                root.GetLocation(),
                "This exception message does not provide context description."));
        }

        private static void AnalyzeGenericExceptionCode(SyntaxNodeAnalysisContext context)
        {
            var root = context.Node as IdentifierNameSyntax;
            var genericExceptions = new[]
            {
                "Exception",
                "ApplicationException",
                "SystemException",
                "ExecutionEngineException",
                "IndexOutOfRangeException",
                "NullReferenceException",
                "OutOfMemoryException"
            };

            if (root == null ||
                genericExceptions.All(x => root.Identifier.ValueText != x) ||
                !(root.Parent is ObjectCreationExpressionSyntax &&
                  root.Parent.Parent is ThrowStatementSyntax))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(
                AggregateExceptionRule,
                root.GetLocation(),
                "A method raises an exception type that is too general or that is reserved by the runtime. Use specific or Aggregation Exception"));
        }

        private static void AnalyzeSwitchWithoutDefaultLabel(SyntaxNodeAnalysisContext context)
        {
            var root = context.Node as SwitchStatementSyntax;

            if (root == null ||
                root.DescendantNodes()
                    .OfType<DefaultSwitchLabelSyntax>().Any())
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(SwitchWithoutDefaultCaseRule, root.GetLocation(),
                "Missing default case for switch"));
        }

        private static void AnalyzeToArrayToListInForeachDeclaration(SyntaxNodeAnalysisContext context)
        {
            var root = context.Node as ForEachStatementSyntax;

            if (root == null)
            {
                return;
            }

            var enumerable = root.Expression.ToString();

            if (!(enumerable.Contains(".ToArray()") ||
                  enumerable.Contains(".ToList()")))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(
                ToArrayToListInsideForeachDeclaration,
                root.Expression.GetLocation(),
                "ToArray/ToList inside foreach declaration. Remove the `ToArray()` or `ToList()` call and use the `IEnumerable` instance directly"));
        }

        private static void AnalyzeLiskovSubstitutionPrincipal(SyntaxNodeAnalysisContext context)
        {
            var predefinedType = context.Node as PredefinedTypeSyntax;
            var parameterType = context.Node as ParameterSyntax;
            var genericType = context.Node as GenericNameSyntax;

            var collectionTypes = new[]
            {
                "Enumerable",
                "ReadOnlyCollection",
                "Collection",
                "ReadOnlyList",
                "Dictionary",
                "List"
            };

            if (predefinedType == null &&
                genericType == null &&
                parameterType == null)
            {
                return;
            }

            if (genericType?.Parent is ObjectCreationExpressionSyntax ||
                genericType?.Parent is TypeOfExpressionSyntax ||
                genericType?.Parent is CastExpressionSyntax ||
                genericType?.Parent is BinaryExpressionSyntax ||
                genericType?.Parent is TypeArgumentListSyntax ||
                genericType?.Parent?.Parent is ArgumentSyntax ||
                predefinedType?.Parent is ObjectCreationExpressionSyntax ||
                predefinedType?.Parent is TypeOfExpressionSyntax ||
                predefinedType?.Parent is BinaryExpressionSyntax ||
                predefinedType?.Parent is TypeArgumentListSyntax ||
                predefinedType?.Parent is CastExpressionSyntax ||
                predefinedType?.Parent?.Parent is ArgumentSyntax ||
                parameterType?.Parent is TypeOfExpressionSyntax)
            {
                return;
            }

            if (CheckPublicMethodOrField(genericType))
            {
                return;
            }

            if (CheckPublicMethodOrField(predefinedType))
            {
                return;
            }

            if (predefinedType?.IsVar == true ||
                genericType?.IsVar == true)
            {
                return;
            }

            if (parameterType != null &&
                !(parameterType.Modifiers.ToString().Contains(PublicModifier) ||
                  parameterType.Modifiers.ToString().Contains(ProtectedModifier)))
            {
                return;
            }

            var type = predefinedType?.ToString() ??
                       parameterType?.Type.ToString() ??
                       genericType?.Identifier.ValueText ??
                       string.Empty;

            if (type.Contains("."))
            {
                type = type
                    .Split('.')
                    .Last();
            }

            if (!collectionTypes.Any(x => type.StartsWith(x)))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(
                LiskovSubsitutionPrincipleRule,
                context.Node.GetLocation(),
                "This member publicly exposes a concrete collection type"));
        }

        private static void AnalyzeContextualKeyWord(SyntaxNodeAnalysisContext context)
        {
            var identifer = context.Node as IdentifierNameSyntax;
            var parameter = context.Node as ParameterSyntax;
            var valueKeyWord = "value";

            if (VerifyContextualKeyword(identifer, identifer?.Identifier.ValueText) &&
                VerifyContextualKeyword(parameter, parameter?.Identifier.ValueText))
            {
                return;
            }

            if (identifer != null &&
                identifer.Ancestors().Any(x => x.IsKind(SyntaxKind.SetAccessorDeclaration)) &&
                identifer.Identifier.ValueText == valueKeyWord)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(
                ContextualKeyWordRule,
                context.Node.GetLocation(),
                "This code uses the contextual keyword as a variable or member name. " +
                "An important improvement would be to rename this variable so that it is not named after a keyword"));
        }

        private static bool VerifyContextualKeyword(SyntaxNode identifer, string text)
        {
            if (identifer == null ||
                identifer.Parent is InvocationExpressionSyntax ||
                identifer.Parent is VariableDeclarationSyntax ||
                identifer.Parent is ForEachStatementSyntax ||
                identifer.Parent is ForStatementSyntax ||
                identifer.Parent is WhileStatementSyntax ||
                identifer.Parent is SwitchStatementSyntax ||
                identifer.Parent is DeclarationExpressionSyntax ||
                identifer.Parent is TypeOfExpressionSyntax ||
                identifer.Parent is MethodDeclarationSyntax ||
                identifer.Parent is ParameterSyntax ||
                identifer.Parent.Parent is GenericNameSyntax ||
                ContextualKeywords.All(x => x != text))
            {
                return true;
            }

            return false;
        }

        public static string[] ContextualKeywords { get; } =
        {
            "add",
            "alias",
            "ascending",
            "async",
            "await",
            "by",
            "descending",
            "dynamic",
            "equals",
            "from",
            "get",
            "global",
            "group",
            "into",
            "join",
            "let",
            "nameof",
            "on",
            "orderby",
            "partial",
            "remove",
            "select",
            "set",
            "value",
            "var",
            "when",
            "where",
            "yield"
        };

        private static void AnalyzeClassesAreNounRule(SyntaxNodeAnalysisContext context)
        {
            var root = context.Node as ClassDeclarationSyntax;
            var keywords = new[]
            {
                "Manager",
                "Processor",
                "Data",
                "Info"
            };

            if (root != null &&
                keywords.Any(x => root.Identifier.ValueText.EndsWith(x)))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    ContextualKeyWordRule,
                    root.GetLocation(),
                    @"Classes are nouns. Rename this class in order to eliminate ""Processor"", ""Data"" or ""Info"" word and " +
                    "keep a high level of expressiveness and meaningfulness."));
            }
        }

        private static void AnalyzeBlankBlockCode(SyntaxNodeAnalysisContext context)
        {
            var root = context.Node as BlockSyntax;

            if (root == null ||
                root.DescendantNodes().Any() ||
                root.Parent is SimpleLambdaExpressionSyntax ||
                root.Parent is LambdaExpressionSyntax ||
                root.Parent is ParenthesizedLambdaExpressionSyntax)
            {
                return;
            }

            var commonMessage =
                "This code has a blank block to do nothing. Sometimes this means the code missed to implement here";

            if (root.Parent is IfStatementSyntax)
            {
                commonMessage = "This method contains an unnecessary empty if statement";
            }
            else if (root.Parent is CatchClauseSyntax)
            {
                commonMessage = @"The exception is ignored (""swallowed"") by the try-catch block.";
            }

            context.ReportDiagnostic(Diagnostic.Create(BlankCodeRule, root.GetLocation(), commonMessage));
        }

        private static void AnalyzeFieldToBeEncapsulate(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is FieldDeclarationSyntax root) ||
                root.Modifiers.ToString().Contains("private") ||
                root.Modifiers.ToString().Contains("internal") ||
                root.Modifiers.ToString().Contains("const") ||
                root.Modifiers.ToString().Contains("static") ||
                string.IsNullOrWhiteSpace(root.Modifiers.ToString()) ||
                root.Declaration.Variables.Count > 1)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(
                EncapsulateFieldRule,
                root.GetLocation(),
                "This code exposes a field as public or protected. Encapsulate this field into a property"));
        }

        private static void AnalyzeMissingParameterNullValidation(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is ParameterSyntax parameterSyntax) ||
                !(parameterSyntax.Parent.Parent is BaseMethodDeclarationSyntax method))
            {
                return;
            }

            if (!(context.SemanticModel.GetDeclaredSymbol(context.Node) is IParameterSymbol parameter))
            {
                return;
            }

            if (parameter.Type.TypeKind == TypeKind.Struct)
            {
                return;
            }

            if (method.Modifiers.ToFullString().Contains("private"))
            {
                return;
            }

            var memberAccessExpressionsOfParameter = method.DescendantNodes().OfType<MemberAccessExpressionSyntax>().Where(m =>
                m.Expression is IdentifierNameSyntax identifier &&
                identifier.Identifier.Text == parameter.Name);
            if (!memberAccessExpressionsOfParameter.Any())
            {
                return;
            }

            if (ThrowsArgumentNullExceptionForParameter(method, parameter, context.SemanticModel))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(
                MissingParameterNullValidationRule,
                parameterSyntax.GetLocation(),
                "An important improvement would be to refactor the block so that it validates the parameter and throws an `ArgumentNullException` rather than letting a `NullReferenceException` occur or any unexpected behavior."));

        }

        private static bool ThrowsArgumentNullExceptionForParameter(BaseMethodDeclarationSyntax method, IParameterSymbol parameter, SemanticModel semanticModel)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            if (semanticModel == null) throw new ArgumentNullException(nameof(semanticModel));

            bool IsArgumentNullExceptionObjectCreationForParameter(ExpressionSyntax expression)
            {
                return expression is ObjectCreationExpressionSyntax objectCreation &&
                       objectCreation.Type is IdentifierNameSyntax identifier &&
                       semanticModel.GetSymbolInfo(identifier).Symbol is INamedTypeSymbol namedType &&
                       namedType.ToDisplayString() == typeof(ArgumentNullException).FullName &&
                       objectCreation.ArgumentList != null &&
                       objectCreation.ArgumentList.Arguments.Any(arg =>
                           arg.Expression.GetText().ToString() == $"nameof({parameter.Name})");
            }

            var hasThrowArgumentNullStatementForNullParameter = method.DescendantNodes().OfType<ThrowStatementSyntax>()
                .Any(t => IsArgumentNullExceptionObjectCreationForParameter(t.Expression));

            var hasThrowArgumentNullExpressionsForNullParameter = method.DescendantNodes().OfType<ThrowExpressionSyntax>()
                .Any(t => IsArgumentNullExceptionObjectCreationForParameter(t.Expression));
            return hasThrowArgumentNullExpressionsForNullParameter || hasThrowArgumentNullStatementForNullParameter;
        }


        private static void AnalyzeMissingConstructorParameterNullValidation(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is ParameterSyntax))
            {
                return;
            }
            var contextNodeSymbol = context.SemanticModel.GetDeclaredSymbol(context.Node);
            if (!(contextNodeSymbol is IParameterSymbol parameter &&
                parameter.ContainingSymbol is IMethodSymbol methodSymbol &&
                methodSymbol.MethodKind == MethodKind.Constructor))
            {
                return;
            }

            if (parameter.Type.TypeKind == TypeKind.Struct)
            {
                return;
            }

            var methodSyntaxNode = methodSymbol.DeclaringSyntaxReferences.First().GetSyntax() as ConstructorDeclarationSyntax;
            if (methodSyntaxNode == null)
            {
                return;
            }

            if (methodSyntaxNode.Modifiers.ToFullString().Contains("private"))
            {
                return;
            }

            var constructorNodes = methodSyntaxNode.DescendantNodes();
            var assignmentsOnFields = constructorNodes.OfType<AssignmentExpressionSyntax>()
                .Where(a => context.SemanticModel.GetSymbolInfo(a.Right).Symbol?.Equals(parameter) == true &&
                            context.SemanticModel.GetSymbolInfo(a.Left).Symbol?.Kind == SymbolKind.Field);

            var fieldsAssignedByParameter =
                assignmentsOnFields.Select(m => context.SemanticModel.GetSymbolInfo(m.Left).Symbol);

            var classMemberAccesses = parameter.ContainingType.DeclaringSyntaxReferences
                .SelectMany(c => c.GetSyntax().DescendantNodes())
                .OfType<MemberAccessExpressionSyntax>()
                .Where(m =>
                {
                    var expressionSymbol = context.SemanticModel.GetSymbolInfo(m.Expression).Symbol;
                    return fieldsAssignedByParameter.Contains(expressionSymbol);
                });

            if (!classMemberAccesses.Any())
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(
                MissingConstructorParameterNullValidationRule,
                context.Node.GetLocation(),
                "An important improvement would be to refactor the block so that it validates the constructor parameter and throws an `ArgumentNullException` rather than letting a `NullReferenceException` occur or any unexpected behavior."));

        }

        private static void AnalyzeParameterNotReassigned(SyntaxNodeAnalysisContext context)
        {
            if (!(context.SemanticModel.GetDeclaredSymbol(context.Node) is IParameterSymbol parameterSymbol))
            {
                return;
            }

            if (parameterSymbol.RefKind != RefKind.None)
            {
                return;
            }

            if (!(parameterSymbol.ContainingSymbol is IMethodSymbol methodSymbol))
            {
                return;
            }

            var methodSyntax = methodSymbol.DeclaringSyntaxReferences.First().GetSyntax();
            var assignments = methodSyntax.DescendantNodes().OfType<AssignmentExpressionSyntax>();

            if (!assignments.Any(assignment =>
                Equals(context.SemanticModel.GetSymbolInfo(assignment.Left).Symbol, parameterSymbol)))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(
                ParameterNotReassignedRule,
                context.Node.GetLocation(),
                "A possible improvement would be to avoid such reassignment and use a local variable to hold the updated value."));
        }

        private static void AnalyzeParameterUnused(SyntaxNodeAnalysisContext context)
        {
            if (!(context.SemanticModel.GetDeclaredSymbol(context.Node) is IParameterSymbol parameterSymbol))
            {
                return;
            }

            if (parameterSymbol.RefKind != RefKind.None)
            {
                return;
            }

            if (!(parameterSymbol.ContainingSymbol is IMethodSymbol methodSymbol))
            {
                return;
            }

            var method = methodSymbol.DeclaringSyntaxReferences.First().GetSyntax() as MethodDeclarationSyntax;

            if (method == null)
            {
                return;
            }

            var dataFlow = context.SemanticModel.AnalyzeDataFlow(method.Body);
            var parametersDeclared = dataFlow.WrittenOutside.Where(x => x.Kind == SymbolKind.Parameter);
            var unused = parametersDeclared.Except(dataFlow.ReadInside);

            foreach (var parameter in unused)
            {
                if (parameter.Name != "this")
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        ParameterUnusedRule,
                        parameter.Locations.First(),
                        $"A possible improvement would be to remove unused parameter {parameter.Name}"));
                }
            }
        }

        private static bool CheckPublicMethodOrField(SyntaxNode genericType)
        {
            var field = genericType?.Ancestors<FieldDeclarationSyntax>().FirstOrDefault();
            var method = genericType?.Ancestors<MethodDeclarationSyntax>().FirstOrDefault();
            var property = genericType?.Ancestors<PropertyDeclarationSyntax>().FirstOrDefault();

            if (field != null)
            {
                if (!(field.Modifiers.ToString().Contains(PublicModifier) ||
                      field.Modifiers.ToString().Contains(ProtectedModifier)))
                {
                    return true;
                }
            }

            if (method != null)
            {
                if (!(method.Modifiers.ToString().Contains(PublicModifier) ||
                      method.Modifiers.ToString().Contains(ProtectedModifier)))
                {
                    return true;
                }
            }

            if (property != null)
            {
                if (!(property.Modifiers.ToString().Contains(PublicModifier) ||
                      property.Modifiers.ToString().Contains(ProtectedModifier)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}