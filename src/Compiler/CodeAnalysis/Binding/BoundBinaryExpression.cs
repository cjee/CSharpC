using Compiler.CodeAnalysis.Symbols;

namespace Compiler.CodeAnalysis.Binding
{
    public class BoundBinaryExpression : BoundExpression
    {
        public BoundBinaryOperator BoundOperator { get; }
        public BoundExpression LeftOperand { get; }
        public BoundExpression RightOperand { get; }

        public BoundBinaryExpression(BoundBinaryOperator boundOperator, BoundExpression leftOperand, BoundExpression rightOperand)
        {
            BoundOperator = boundOperator;
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
        }

        public override BoundNodeKind Kind => BoundNodeKind.BinaryExpression;
        public override TypeSymbol Type => BoundOperator.ResultType;
    }
}