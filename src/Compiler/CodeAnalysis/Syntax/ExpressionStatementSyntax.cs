using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed record ExpressionStatementSyntax(ExpressionSyntax Expression, SyntaxNode Semicolon) : StatementSyntax
    {
        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Expression;
            yield return Semicolon;
        }
    }
}