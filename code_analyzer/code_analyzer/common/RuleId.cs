using Microsoft.CodeAnalysis;

namespace code_analyzer.common
{
    public static class RuleId
    {
        public const string EncapsulateFieldRuleId = "encapsulate_public_protected_field";
        public const string ContextualKeywordRuleId = "avoid_contextual_keywords";
        public const string BlankBlockCodeRuleId = "blank_block_code";
        public const string LiskovSubstitutionPrincipleRuleId = "liskov_substitution_principle";
        public const string ToArrayToListInsideForeachDeclarationRuleId = "toarray_tolist_inside_foreach_declaration";
        public const string SwitchWithoutDefaultCaseRuleId = "switch_without_default_case";
        public const string AggregateExceptionRuleId = "aggregate_exception";
        public const string ExceptionWithNoContextRuleId = "exception_without_context";
        public const string EnumDefaultValueRuleId = "enum_without_default_value";
        public const string MethodWithMoreThanFourParamtersRuleId = "method_with_more_than_four_paramters";
        public const string MethodWithBoolAsParameterRuleId = "method_with_bool_as_parameter";
        public const string ClassOverStructRuleId = "prefer_class_over_struct";

        public static LocalizableString Get(this string resource)
        {
            return new LocalizableResourceString(
                resource,
                Resources.ResourceManager, typeof(Resources));
        }
    }
}