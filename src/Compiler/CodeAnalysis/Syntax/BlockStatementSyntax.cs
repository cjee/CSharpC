using System.Collections.Generic;
using System.Collections.Immutable;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed record BlockStatementSyntax(SyntaxToken OpenBrace, ImmutableList<StatementSyntax> Statements, SyntaxToken CloseBrace) : StatementSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.BlockStatement;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OpenBrace;
            foreach (var statement in Statements)
            {
                yield return statement;
            }
            yield return CloseBrace;
        }
    }
}