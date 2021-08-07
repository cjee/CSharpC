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
        
        OpenParenthesisToken,
        CloseParenthesisToken,
        OpenBraceToken,
        CloseBraceToken,
        SemicolonToken,
        DotToken,
        CommaToken,

        // Keyworkds
        FalseKeyword,
        TrueKeyword,
        VoidKeyword,
        BoolKeyword,
        IntKeyword,
        ReturnKeyword,

        Identifier,
        Type,

        // Expressions
        EmptyExpression,
        NumericLiteralExpression,
        BooleanLiteralExpression,
        SimpleNameExpression,
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression,
        InvocationExpression,

        // Statements
        BlockStatement,
        EmptyStatement,
        LocalVariableDeclarationStatement,
        ExpressionStatement,
        ReturnStatement,
        
        //Member syntax
        MethodDeclaration,
        Parameter,
        
        CompilationUnit,
    }
}