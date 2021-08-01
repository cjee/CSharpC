using Compiler.CodeAnalysis.Symbols;

namespace Compiler.CodeAnalysis.Binding
{
    internal class BoundIntegralLiteralExpression : BoundExpression
    {
        public int Value { get; }

        public BoundIntegralLiteralExpression(int value)
        {
            Value = value;
        }

        public override BoundNodeKind Kind => BoundNodeKind.IntegralLiteralExpression;
        public override TypeSymbol Type => TypeSymbol.Int;
    }
}