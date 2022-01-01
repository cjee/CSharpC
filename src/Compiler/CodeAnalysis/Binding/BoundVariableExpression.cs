using Compiler.CodeAnalysis.Symbols;

namespace Compiler.CodeAnalysis.Binding;

internal class BoundVariableExpression : BoundExpression
{
    public VariableSymbol Variable { get; }

    public BoundVariableExpression(VariableSymbol variable)
    {
        Variable = variable;
    }

    public override BoundNodeKind Kind => BoundNodeKind.VariableExpression;
    public override TypeSymbol Type => Variable.Type;
}