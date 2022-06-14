using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed record UnaryExpressionSyntax(SyntaxToken OperatorToken, ExpressionSyntax Operand) : ExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.UnaryExpression;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OperatorToken;
            yield return Operand;
        }
    }
}