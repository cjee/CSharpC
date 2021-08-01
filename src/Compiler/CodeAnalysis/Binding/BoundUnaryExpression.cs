using Compiler.CodeAnalysis.Symbols;

namespace Compiler.CodeAnalysis.Binding
{
    public class BoundUnaryExpression : BoundExpression
    {
        public BoundUnaryOperator BoundOperator { get; }
        public BoundExpression BoundOperand { get; }

        public BoundUnaryExpression(BoundUnaryOperator boundOperator, BoundExpression boundOperand)
        {
            BoundOperator = boundOperator;
            BoundOperand = boundOperand;
        }

        public override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;
        public override TypeSymbol Type => BoundOperator.ResultType;
    }
}