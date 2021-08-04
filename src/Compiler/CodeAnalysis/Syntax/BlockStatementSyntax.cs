using System.Collections.Generic;
using System.Collections.Immutable;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed class BlockStatementSyntax : StatementSyntax
    {
        public SyntaxToken OpenBrace { get; }
        public ImmutableList<StatementSyntax> Statements { get; }
        public SyntaxToken CloseBrace { get; }

        public BlockStatementSyntax(SyntaxToken openBrace, ImmutableList<StatementSyntax> statements, SyntaxToken closeBrace)
        {
            OpenBrace = openBrace;
            Statements = statements;
            CloseBrace = closeBrace;
        }

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