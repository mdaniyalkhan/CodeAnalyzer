using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace code_analyzer.common
{
    public static class SyntaxExtension
    {
        /// <summary>
        /// Descendant Nodes
        /// </summary>
        public static IList<T> DescendantNodes<T>(this SyntaxNode syntaxNode)
        {
            return syntaxNode?
                .DescendantNodes()?
                .OfType<T>()
                .ToList();
        }

        /// <summary>
        /// Ancestors
        /// </summary>
        public static IList<T> Ancestors<T>(this SyntaxNode syntaxNode)
        {
            return syntaxNode?
                .Ancestors()?
                .OfType<T>()
                .ToList();
        }
    }
}
