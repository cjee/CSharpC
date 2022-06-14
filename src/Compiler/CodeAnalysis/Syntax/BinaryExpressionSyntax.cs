using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed record BinaryExpressionSyntax(ExpressionSyntax Left, SyntaxToken OperatorToken, ExpressionSyntax Right) : ExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.BinaryExpression;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Left;
            yield return OperatorToken;
            yield return Right;
        }
    }
}