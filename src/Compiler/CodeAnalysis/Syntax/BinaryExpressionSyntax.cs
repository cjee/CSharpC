using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed class BinaryExpressionSyntax : ExpressionsSyntax
    {
        public BinaryExpressionSyntax(ExpressionsSyntax left, SyntaxToken operatorToken, ExpressionsSyntax right)
        {
            Left = left;
            OperatorToken = operatorToken;
            Right = right;
        }

        public ExpressionsSyntax Left { get; }
        public SyntaxToken OperatorToken { get; }
        public ExpressionsSyntax Right { get; }

        public override SyntaxKind Kind => SyntaxKind.BinaryExpression;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Left;
            yield return OperatorToken;
            yield return Right;
        }
    }
}