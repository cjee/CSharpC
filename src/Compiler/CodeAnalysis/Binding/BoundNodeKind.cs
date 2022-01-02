namespace Compiler.CodeAnalysis.Binding
{
    public enum BoundNodeKind
    {
        ErrorExpression,
        IntegralLiteralExpression,
        BooleanLiteralExpression,
        UnaryExpression,
        BinaryExpression,
        VariableExpression,
        
        //Statements
        BoundBlockStatement,
        BoundEmptyStatement,
        BoundLocalVariableDeclarationStatement,
        BoundExpressionStatement,
        AssignmentExpression,
        InvocationExpression,
        ReturnStatement,
    }
}