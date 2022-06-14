using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed record ReturnStatementSyntax(SyntaxToken ReturnKeyword, ExpressionSyntax? Expression, SyntaxToken Semicolon) : StatementSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.ReturnStatement;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return ReturnKeyword;
            if (Expression is not null)
                yield return Expression;
            yield return Semicolon;
        }
    }
}