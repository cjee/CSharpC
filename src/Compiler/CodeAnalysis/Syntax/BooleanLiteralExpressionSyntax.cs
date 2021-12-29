using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed class BooleanLiteralExpressionSyntax : ExpressionSyntax
    {
        public SyntaxToken BooleanToken { get; }

        public BooleanLiteralExpressionSyntax(SyntaxToken booleanToken)
        {
            BooleanToken = booleanToken;
        }

        public override SyntaxKind Kind => SyntaxKind.BooleanLiteralExpression;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return BooleanToken;
        }
    }
}