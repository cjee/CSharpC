using System.Collections.Generic;
using System.Linq;
using Compiler.CodeAnalysis.Text;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed record SyntaxToken(SyntaxKind Kind, int Position, string Text, object? Value) : SyntaxNode
    {
        public override SyntaxKind Kind { get; } = Kind;

        public TextSpan TextSpan => new(Position, Text.Length);

        public override TextSpan Span => TextSpan;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }

        public override string ToString()
        {
            return $"Pos: {Position}: Type: {Kind}: text: '{Text}'";
        }
    }
}