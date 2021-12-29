using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed class NumericLiteralExpressionSyntax : ExpressionSyntax
    {
        public NumericLiteralExpressionSyntax(SyntaxToken numberToken)
        {
            NumberToken = numberToken;
        }

        public SyntaxToken NumberToken { get; }

        public override SyntaxKind Kind => SyntaxKind.NumericLiteralExpression;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return NumberToken;
        }
    }
}