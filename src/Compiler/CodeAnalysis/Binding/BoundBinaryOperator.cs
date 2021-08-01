using System.Linq;
using Compiler.CodeAnalysis.Symbols;
using Compiler.CodeAnalysis.Syntax;

namespace Compiler.CodeAnalysis.Binding
{
    public class BoundBinaryOperator 
    {
        public SyntaxKind SyntaxKind { get; }
        public BoundBinaryOperatorKind BoundUnaryOperatorKind { get; }
        public TypeSymbol LeftType { get; }
        public TypeSymbol RightType { get; }
        public TypeSymbol ResultType { get; }

        public BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind boundUnaryOperatorKind, TypeSymbol type)
            : this(syntaxKind, boundUnaryOperatorKind, type, type, type )
        {
        }
        
        public BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind boundUnaryOperatorKind, TypeSymbol operandType, TypeSymbol resultType)
            : this(syntaxKind, boundUnaryOperatorKind, operandType, operandType, resultType)
        {
        }
        
        public BoundBinaryOperator(
            SyntaxKind syntaxKind, 
            BoundBinaryOperatorKind boundUnaryOperatorKind, 
            TypeSymbol leftType,
            TypeSymbol rightType,
            TypeSymbol resultType)
        {
            SyntaxKind = syntaxKind;
            BoundUnaryOperatorKind = boundUnaryOperatorKind;
            LeftType = leftType;
            RightType = rightType;
            ResultType = resultType;
        }

        private static BoundBinaryOperator[] operators =
        {
            new BoundBinaryOperator(SyntaxKind.PlusToken, BoundBinaryOperatorKind.Addition, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxKind.MinusToken, BoundBinaryOperatorKind.Subtraction, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxKind.StarToken, BoundBinaryOperatorKind.Multiplication, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxKind.SlashToken, BoundBinaryOperatorKind.Division, TypeSymbol.Int),
        };

        public static BoundBinaryOperator? Bind(SyntaxKind operandKind, TypeSymbol leftOperandType, TypeSymbol rightOperandType)
        {
            var binaryOperator =
                operators.FirstOrDefault(x =>
                    x.SyntaxKind == operandKind && x.LeftType == leftOperandType && x.RightType == rightOperandType);

            return binaryOperator;
        }
    
    
    }
}