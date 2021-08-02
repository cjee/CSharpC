using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed class EmptyStatement : Statement
    {
        public SyntaxToken Semicolon { get; }

        public EmptyStatement(SyntaxToken semicolon)
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