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
        
        public static int GetBinaryOperatorPrecedence(SyntaxKind currentKind)
        {
            switch (currentKind)
            {
                case SyntaxKind.StarToken:
                case SyntaxKind.SlashToken:
                    return 2;
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                    return 1;
                default:
                    return 0;
            }
        }
    }
}