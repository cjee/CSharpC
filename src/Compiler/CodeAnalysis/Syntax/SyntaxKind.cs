namespace Compiler.CodeAnalysis.Syntax
{
    public enum SyntaxKind
    {
        BadToken,
        EndOfFileToken,
        WhitespaceToken,
        IntegerLiteralToken,
        
        PlusToken,
        MinusToken,
        StarToken,
        SlashToken,
        PercentToken,
        
        BangToken,
        
        LessToken,
        LessOrEqualToken,
        GreaterToken,
        GreaterOrEqualToken,
        
        BangEqualsToken,
        EqualsEqualsToken,
        
        EqualsToken,
        
        OpenParenthesis,
        CloseParenthesis,

        //Keyworkds
        FalseKeyword,
        TrueKeyword,

        NumericLiteralExpression,
        BooleanLiteralExpression,
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression,
    }
}