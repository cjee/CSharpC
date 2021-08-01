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
                SyntaxKind.OpenParenthesis => "(",
                SyntaxKind.CloseParenthesis => ")",
                _ => string.Empty,
            };
        }
        
        public static int GetUnaryOperatorPrecedence(this SyntaxKind currentKind)
        {
            switch (currentKind)
            {
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                    return 13; //Based on C# operator count
                default:
                    return 0;
            }
        }
        
        public static int GetBinaryOperatorPrecedence(this SyntaxKind currentKind)
        {
            switch (currentKind)
            {
                case SyntaxKind.StarToken:
                case SyntaxKind.SlashToken:
                    return 12;
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                    return 11;
                default:
                    return 0;
            }
        }
    }
}