using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed class SimpleNameExpressionSyntax : ExpressionsSyntax
    {
        public SyntaxToken Identifier { get; }

        public SimpleNameExpressionSyntax(SyntaxToken identifier)
        {
            Identifier = identifier;
        }

        public override SyntaxKind Kind => SyntaxKind.SimpleNameExpression;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Identifier;
        }
    }
}