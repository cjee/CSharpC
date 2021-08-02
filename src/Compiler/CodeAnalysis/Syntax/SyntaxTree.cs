namespace Compiler.CodeAnalysis.Syntax
{
    public class SyntaxTree
    {
        public SyntaxTree(DiagnosticBag diagnostics, BlockStatement root, SyntaxToken endOfFileToken)
        {
            Diagnostics = diagnostics;
            Root = root;
            EndOfFileToken = endOfFileToken;
        }

        public DiagnosticBag Diagnostics { get; }
        public BlockStatement Root { get; }
        public SyntaxToken EndOfFileToken { get; }

        public static SyntaxTree Parse(string text)
        {
            Parser parser = new(text);
            return parser.Parse();
        }
    }
}