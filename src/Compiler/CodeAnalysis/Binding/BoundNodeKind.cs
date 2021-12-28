namespace Compiler.CodeAnalysis.Binding
{
    public enum BoundNodeKind
    {
        IntegralLiteralExpression,
        BooleanLiteralExpression,
        UnaryExpression,
        BinaryExpression,
        
        //Statements
        BoundBlockStatement,
        BoundEmptyStatement,
        BoundLocalVariableDeclarationStatement,
    }
}