using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed record BooleanLiteralExpressionSyntax(SyntaxToken BooleanToken) : ExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.BooleanLiteralExpression;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return BooleanToken;
        }
    }
}