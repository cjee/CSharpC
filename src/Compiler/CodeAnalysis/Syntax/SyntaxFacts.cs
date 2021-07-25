namespace Compiler.CodeAnalysis.Syntax
{
    public static class SyntaxFacts
    {
        public static string GetText(SyntaxKind kind)
        {
            return kind switch
            {
                SyntaxKind.PlusToken => "+",
                SyntaxKind.MinusToken => "-",
                SyntaxKind.StarToken => "*",
                SyntaxKind.SlashToken => "/",
                _ => string.Empty,
            };
        }
    }
}