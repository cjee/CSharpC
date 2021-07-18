namespace Compiler
{
    public class SyntaxToken : SyntaxNode
    {
        public SyntaxKind Kind { get; }
        public int Position { get; }
        public string Text { get; }
        public object Value { get; }

        public TextSpan TextSpan => new (Position, Text?.Length ?? 0);
        
        public SyntaxToken(SyntaxKind kind, int position, string text, object value)
        {
            Kind = kind;
            Position = position;
            Text = text;
            Value = value;
        }

        public override string ToString()
        {
            return $"Pos: {Position}: Type: {Kind}: text: '{Text}'";
        }
    }
}