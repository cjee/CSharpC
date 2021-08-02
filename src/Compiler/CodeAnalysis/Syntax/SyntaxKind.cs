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
        OpenBrace,
        CloseBrace,
        Semicolon,

        //Keyworkds
        FalseKeyword,
        TrueKeyword,
        
        BoolKeyword,
        IntKeyword,

        NumericLiteralExpression,
        BooleanLiteralExpression,
        
        Identifier,
        
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression,

        //Statements
        BlockStatement,
        EmptyStatement,
    }
}