using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed class EmptyStatementSyntaxSyntax : StatementSyntax
    {
        public SyntaxToken Semicolon { get; }

        public EmptyStatementSyntaxSyntax(SyntaxToken semicolon)
        {
            Semicolon = semicolon;
        }

        public override SyntaxKind Kind => SyntaxKind.EmptyStatement;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Semicolon;
        }
    }
}