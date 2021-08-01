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