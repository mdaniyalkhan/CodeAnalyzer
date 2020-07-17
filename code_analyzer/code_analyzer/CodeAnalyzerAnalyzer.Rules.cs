using code_analyzer.common;
using Microsoft.CodeAnalysis;

namespace code_analyzer
{
    public partial class CodeAnalyzerAnalyzer
    {
        private static readonly DiagnosticDescriptor DuplicateShims = new DiagnosticDescriptor(
            RuleId.DuplicateShims,
            nameof(Resources.DuplicateShimsTitle).Get(),
            nameof(Resources.MessageFormat).Get(),
            Category,
            DiagnosticSeverity.Warning,
            true);

        private static readonly DiagnosticDescriptor SimplifyShims = new DiagnosticDescriptor(
            RuleId.SimplifyShims,
            nameof(Resources.SimplifyShimsTitle).Get(),
            nameof(Resources.MessageFormat).Get(),
            Category,
            DiagnosticSeverity.Warning,
            true);

        private static readonly DiagnosticDescriptor UnnecessaryShimsContext = new DiagnosticDescriptor(
            RuleId.UnnecessaryShimsContext,
            nameof(Resources.UnnecessaryShimsContextTitle).Get(),
            nameof(Resources.MessageFormat).Get(),
            Category,
            DiagnosticSeverity.Warning,
            true);

        private static readonly DiagnosticDescriptor ReplaceMagicValues = new DiagnosticDescriptor(
            RuleId.ReplaceMagicValues,
            nameof(Resources.ReplaceMagicValuesTitle).Get(),
            nameof(Resources.MessageFormat).Get(),
            Category,
            DiagnosticSeverity.Info,
            true);

        private static readonly DiagnosticDescriptor EncapsulateFieldRule = new DiagnosticDescriptor(
            RuleId.EncapsulateFieldRuleId,
            nameof(Resources.EncapsulateFieldTitle).Get(),
            nameof(Resources.MessageFormat).Get(),
            Category,
            DiagnosticSeverity.Warning,
            true);

        private static readonly DiagnosticDescriptor ContextualKeyWordRule = new DiagnosticDescriptor(
            RuleId.ContextualKeywordRuleId,
            nameof(Resources.ContextualKeywordsTitle).Get(),
            nameof(Resources.MessageFormat).Get(),
            Category,
            DiagnosticSeverity.Warning,
            true);

        private static readonly DiagnosticDescriptor BlankCodeRule = new DiagnosticDescriptor(
            RuleId.BlankBlockCodeRuleId,
            nameof(Resources.BlankBlockCodeTitle).Get(),
            nameof(Resources.MessageFormat).Get(),
            Category,
            DiagnosticSeverity.Warning,
            true);

        private static readonly DiagnosticDescriptor LiskovSubsitutionPrincipleRule = new DiagnosticDescriptor(
            RuleId.LiskovSubstitutionPrincipleRuleId,
            nameof(Resources.LiskovSubsitutionPrincipalTitle).Get(),
            nameof(Resources.MessageFormat).Get(),
            Category,
            DiagnosticSeverity.Warning,
            true);

        private static readonly DiagnosticDescriptor ToArrayToListInsideForeachDeclaration = new DiagnosticDescriptor(
            RuleId.ToArrayToListInsideForeachDeclarationRuleId,
            nameof(Resources.ToArrayToListInsideForeachDeclarationTitle).Get(),
            nameof(Resources.MessageFormat).Get(),
            Category,
            DiagnosticSeverity.Warning,
            true);

        private static readonly DiagnosticDescriptor SwitchWithoutDefaultCaseRule = new DiagnosticDescriptor(
            RuleId.SwitchWithoutDefaultCaseRuleId,
            nameof(Resources.SwitchWihoutDefaultCaseTitle).Get(),
            nameof(Resources.MessageFormat).Get(),
            Category,
            DiagnosticSeverity.Warning,
            true);

        private static readonly DiagnosticDescriptor AggregateExceptionRule = new DiagnosticDescriptor(
            RuleId.AggregateExceptionRuleId,
            nameof(Resources.AggregateExceptionTitle).Get(),
            nameof(Resources.MessageFormat).Get(),
            Category,
            DiagnosticSeverity.Warning,
            true);

        private static readonly DiagnosticDescriptor ExceptionWithoutContextRule = new DiagnosticDescriptor(
            RuleId.ExceptionWithNoContextRuleId,
            nameof(Resources.ExceptionWithoutContextTitle).Get(),
            nameof(Resources.MessageFormat).Get(),
            Category,
            DiagnosticSeverity.Warning,
            true);

        private static readonly DiagnosticDescriptor EnumDefaultValueRule = new DiagnosticDescriptor(
            RuleId.EnumDefaultValueRuleId,
            nameof(Resources.EnumDefaultValueTitle).Get(),
            nameof(Resources.MessageFormat).Get(),
            Category,
            DiagnosticSeverity.Warning,
            true);

        private static readonly DiagnosticDescriptor MethodWithMoreThanSevenParametersRule = new DiagnosticDescriptor(
            RuleId.MethodWithMoreThanSevenParamtersRuleId,
            nameof(Resources.MethodWithMoreThanSevenParametersTitle).Get(),
            nameof(Resources.MessageFormat).Get(),
            Category,
            DiagnosticSeverity.Warning,
            true);

        private static readonly DiagnosticDescriptor MethodWithBoolAsParameterRule = new DiagnosticDescriptor(
            RuleId.MethodWithBoolAsParameterRuleId,
            nameof(Resources.MethodWithBoolAsParameterTitle).Get(),
            nameof(Resources.MessageFormat).Get(),
            Category,
            DiagnosticSeverity.Warning,
            true);

        private static readonly DiagnosticDescriptor PreferClassOverStructRule = new DiagnosticDescriptor(
            RuleId.ClassOverStructRuleId,
            nameof(Resources.PreferClassOverStructTitle).Get(),
            nameof(Resources.MessageFormat).Get(),
            Category,
            DiagnosticSeverity.Warning,
            true);

        private static readonly DiagnosticDescriptor InappropriateUsageOfPropertyRule = new DiagnosticDescriptor(
            RuleId.InappropriateUsageOfPropertyRuleId,
            nameof(Resources.InappropriateUsageOfPropertyTitle).Get(),
            nameof(Resources.MessageFormat).Get(),
            Category,
            DiagnosticSeverity.Warning,
            true);

        private static readonly DiagnosticDescriptor ShouldlySingleAssertInUowRule = new DiagnosticDescriptor(
            RuleId.ShouldlySingleAssertInUowRuleId,
            nameof(Resources.ShouldlySingleAssertInUowTitle).Get(),
            nameof(Resources.MessageFormat).Get(),
            Category,
            DiagnosticSeverity.Warning,
            true);

        private static readonly DiagnosticDescriptor NonPrivateConstantsRule = new DiagnosticDescriptor(
            RuleId.NonPrivateConstantsRuleId,
            nameof(Resources.NonPrivateConstantsTitle).Get(),
            nameof(Resources.MessageFormat).Get(),
            Category,
            DiagnosticSeverity.Warning,
            true);

        private static readonly DiagnosticDescriptor TestCaseArgumentsRule = new DiagnosticDescriptor(
            RuleId.TestCasesArgumentsRuleId,
            nameof(Resources.TestCaseArgumentsTitle).Get(),
            nameof(Resources.MessageFormat).Get(),
            Category,
            DiagnosticSeverity.Warning,
            true);

        private static readonly DiagnosticDescriptor MissingParameterNullValidationRule = new DiagnosticDescriptor(
            RuleId.MissingParameterNullValidation,
            nameof(Resources.MissingParameterNullValidationTitle).Get(),
            nameof(Resources.MessageFormat).Get(),
            Category,
            DiagnosticSeverity.Warning,
            true);

        private static readonly DiagnosticDescriptor MissingConstructorParameterNullValidationRule = new DiagnosticDescriptor(
            RuleId.MissingConstructorParameterNullValidation,
            nameof(Resources.MissingConstructorParameterNullValidationTitle).Get(),
            nameof(Resources.MessageFormat).Get(),
            Category,
            DiagnosticSeverity.Warning,
            true);
    }
}