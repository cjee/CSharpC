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

        NumericLiteralExpression,
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression,
    }
}