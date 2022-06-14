using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed record NumericLiteralExpressionSyntax(SyntaxToken NumberToken) : ExpressionSyntax
    {
        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return NumberToken;
        }
    }
}