using code_analyzer.common;
using Microsoft.CodeAnalysis;

namespace code_analyzer
{
    public partial class CodeAnalyzerAnalyzer
    {
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

        private static readonly DiagnosticDescriptor MethodWithMoreThanFourParamtersRule = new DiagnosticDescriptor(
            RuleId.MethodWithMoreThanFourParamtersRuleId,
            nameof(Resources.MethodWithMoreThanFourParametersTitle).Get(),
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
    }
}