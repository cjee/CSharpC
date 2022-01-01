using System.Collections.Immutable;
using Compiler.CodeAnalysis.Symbols;

namespace Compiler.CodeAnalysis.Binding;

internal class BoundInvocationExpression : BoundExpression
{
    public MethodSymbol Method { get; }
    public ImmutableList<BoundExpression> Arguments { get; }

    public BoundInvocationExpression(MethodSymbol method, ImmutableList<BoundExpression> arguments)
    {
        Method = method;
        Arguments = arguments;
    }

    public override BoundNodeKind Kind => BoundNodeKind.InvocationExpression;
    public override TypeSymbol Type => Method.Type;
}