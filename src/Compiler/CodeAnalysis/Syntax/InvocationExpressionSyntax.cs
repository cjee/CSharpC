using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed record InvocationExpressionSyntax(
            ExpressionSyntax PrimaryExpression,
            SyntaxToken OpenParenthesis,
            SeperatedSyntaxList<ExpressionSyntax> Arguments,
            SyntaxToken CloseParenthesis) : ExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.InvocationExpression;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return PrimaryExpression;
            yield return OpenParenthesis;
            foreach (var argument in Arguments)
            {
                yield return argument;
            }
            yield return CloseParenthesis;
        }
    }
}