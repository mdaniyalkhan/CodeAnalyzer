using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace code_analyzer.common
{
    public class ConstantMapper
    {
        public string ConstantName { get; set; }

        public LiteralExpressionSyntax Literal { get; set; }
    }
}
