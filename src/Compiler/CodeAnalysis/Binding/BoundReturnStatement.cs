namespace Compiler.CodeAnalysis.Binding;

internal class BoundReturnStatement : BoundStatment
{
    public BoundExpression? BoundExpression { get; }

    public BoundReturnStatement(BoundExpression? boundExpression)
    {
        BoundExpression = boundExpression;
    }

    public override BoundNodeKind Kind => BoundNodeKind.ReturnStatement;
}