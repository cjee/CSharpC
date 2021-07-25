using System.Collections.Generic;
using System.Linq;
using Compiler.CodeAnalysis.Text;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed class SyntaxToken : SyntaxNode
    {
        public SyntaxToken(SyntaxKind kind, int position, string text, object? value)
        {
            Kind = kind;
            Position = position;
            Text = text;
            Value = value;
        }

        public override SyntaxKind Kind { get; }

        public int Position { get; }
        public string Text { get; }
        public object? Value { get; }

        public TextSpan TextSpan => new(Position, Text.Length);

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