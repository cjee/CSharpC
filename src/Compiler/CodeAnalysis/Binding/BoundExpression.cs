using Compiler.CodeAnalysis.Symbols;
using System.Collections.Immutable;

namespace Compiler.CodeAnalysis.Binding;

public abstract record BoundExpression : BoundNode
{
    public abstract TypeSymbol Type { get; }
}
public sealed record BoundAssignmentExpression(VariableSymbol Variable, BoundExpression Expression) : BoundExpression
{
    public override TypeSymbol Type => Expression.Type;
}
public sealed record BoundBinaryExpression(BoundBinaryOperator BoundOperator, BoundExpression LeftOperand, BoundExpression RightOperand) : BoundExpression
{
    public override TypeSymbol Type => BoundOperator.ResultType;
}
public sealed record BoundBooleanLiteralExpression(bool Value) : BoundExpression
{
    public override TypeSymbol Type => TypeSymbols.Boolean;
}
public sealed record BoundErrorExpression : BoundExpression
{
    public override TypeSymbol Type => TypeSymbols.Error;
}
public sealed record BoundIntegralLiteralExpression(int Value) : BoundExpression
{
    public override TypeSymbol Type => TypeSymbols.Int;
}
public sealed record BoundInvocationExpression(MethodSymbol Method, ImmutableList<BoundExpression> Arguments) : BoundExpression
{
    public override TypeSymbol Type => Method.Type;
}
public sealed record BoundUnaryExpression(BoundUnaryOperator BoundOperator, BoundExpression BoundOperand) : BoundExpression
{
    public override TypeSymbol Type => BoundOperator.ResultType;
}
public sealed record BoundVariableExpression(VariableSymbol Variable) : BoundExpression
{
    public override TypeSymbol Type => Variable.Type;
}