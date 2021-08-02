using System.Collections.Generic;
using System.Collections.Immutable;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed class BlockStatement : Statement
    {
        public SyntaxToken OpenBrace { get; }
        public ImmutableList<Statement> Statements { get; }
        public SyntaxToken CloseBrace { get; }

        public BlockStatement(SyntaxToken openBrace, ImmutableList<Statement> statements, SyntaxToken closeBrace)
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