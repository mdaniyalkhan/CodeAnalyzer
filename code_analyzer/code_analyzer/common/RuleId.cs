using Microsoft.CodeAnalysis;

namespace code_analyzer.common
{
    public static class RuleId
    {
        public static readonly string ReplaceMagicValues = "replace_magic_values";
        public static readonly string UnnecessaryShimsContext = "unnecessary_shims_context";
        public static readonly string EncapsulateFieldRuleId = "encapsulate_public_protected_field";
        public static readonly string ContextualKeywordRuleId = "avoid_contextual_keywords";
        public static readonly string BlankBlockCodeRuleId = "blank_block_code";
        public static readonly string LiskovSubstitutionPrincipleRuleId = "liskov_substitution_principle";
        public static readonly string ToArrayToListInsideForeachDeclarationRuleId = "toarray_tolist_inside_foreach_declaration";
        public static readonly string SwitchWithoutDefaultCaseRuleId = "switch_without_default_case";
        public static readonly string AggregateExceptionRuleId = "aggregate_exception";
        public static readonly string ExceptionWithNoContextRuleId = "exception_without_context";
        public static readonly string EnumDefaultValueRuleId = "enum_without_default_value";
        public static readonly string MethodWithMoreThanSevenParamtersRuleId = "method_with_more_than_seven_parameters";
        public static readonly string MethodWithBoolAsParameterRuleId = "method_with_bool_as_parameter";
        public static readonly string ClassOverStructRuleId = "prefer_class_over_struct";
        public static readonly string InappropriateUsageOfPropertyRuleId = "inappropriate_usage_of_property";
        public static readonly string ShouldlySingleAssertInUowRuleId = "shouldly_single_assert_uow";
        public static readonly string NonPrivateConstantsRuleId = "non_private_constants";
        public static readonly string TestCasesArgumentsRuleId = "test_case_args";
        public static readonly string SimplifyShims = "simplify_shims";
        public static readonly string DuplicateShims = "duplicate_shims";

        public static LocalizableString Get(this string resource)
        {
            return new LocalizableResourceString(
                resource,
                Resources.ResourceManager, typeof(Resources));
        }
    }
}