using System.Collections.Generic;
using System.Linq;
using Compiler.CodeAnalysis.Text;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed record SyntaxToken(SyntaxKind TokenKind, int Position, string Text, object? Value) : SyntaxNode
    {
        public override SyntaxKind Kind => TokenKind;

        public TextSpan TextSpan => new(Position, Text.Length);

        public override TextSpan Span => TextSpan;

        public override string ToString()
        {
            return $"Pos: {Position}: Type: {Kind}: text: '{Text}'";
        }
    }
}