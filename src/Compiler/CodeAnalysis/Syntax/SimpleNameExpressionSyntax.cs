using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed record SimpleNameExpressionSyntax(SyntaxToken Identifier) : ExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.SimpleNameExpression;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Identifier;
        }
    }
}