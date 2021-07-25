using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed class UnaryExpressionSyntax : ExpressionsSyntax
    {
        public UnaryExpressionSyntax(SyntaxToken operatorToken, ExpressionsSyntax operand)
        {
            OperatorToken = operatorToken;
            Operand = operand;
        }

        public SyntaxToken OperatorToken { get; }
        public ExpressionsSyntax Operand { get; }
        public override SyntaxKind Kind => SyntaxKind.UnaryExpression;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OperatorToken;
            yield return Operand;
        }
    }
}