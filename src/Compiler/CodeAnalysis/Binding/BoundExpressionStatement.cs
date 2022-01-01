namespace Compiler.CodeAnalysis.Binding;

internal class BoundExpressionStatement : BoundStatment
{
    public BoundExpression Expression { get; }

    public BoundExpressionStatement(BoundExpression expression)
    {
        Expression = expression;
    }

    public override BoundNodeKind Kind => BoundNodeKind.BoundExpressionStatement;
}