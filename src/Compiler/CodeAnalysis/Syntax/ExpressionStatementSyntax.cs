using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed class ExpressionStatementSyntax : StatementSyntax
    {
        public ExpressionSyntax Expression { get; }
        public SyntaxNode Semicolon { get; }

        public ExpressionStatementSyntax(ExpressionSyntax expression, SyntaxNode semicolon)
        {
            Expression = expression;
            Semicolon = semicolon;
        }

        public override SyntaxKind Kind => SyntaxKind.ExpressionStatement;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Expression;
            yield return Semicolon;
        }
    }
}