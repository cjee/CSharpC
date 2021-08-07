using System.Runtime.CompilerServices;

namespace Compiler.CodeAnalysis.Syntax
{
    public class SyntaxTree
    {
        public SyntaxTree(DiagnosticBag diagnostics, CompilationUnit root)
        {
            Diagnostics = diagnostics;
            Root = root;
        }

        public DiagnosticBag Diagnostics { get; }
        public CompilationUnit Root { get; }

        public static SyntaxTree Parse(string text)
        {
            Parser parser = new(text);
            return parser.Parse();
        }
    }
}