namespace Compiler.CodeAnalysis.Binding;

internal class BoundEmptyStatement : BoundStatment
{
    public override BoundNodeKind Kind => BoundNodeKind.BoundEmptyStatement;
}