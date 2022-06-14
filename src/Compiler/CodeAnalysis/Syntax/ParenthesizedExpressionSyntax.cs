using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed record ParenthesizedExpressionSyntax(
            SyntaxToken OpenParenthesis,
            ExpressionSyntax Expression,
            SyntaxToken CloseParenthesis) : ExpressionSyntax
    {
        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OpenParenthesis;
            yield return Expression;
            yield return CloseParenthesis;
        }
    }
}