using System.Linq;
using Compiler.CodeAnalysis.Symbols;
using Compiler.CodeAnalysis.Syntax;

namespace Compiler.CodeAnalysis.Binding
{
    public class BoundBinaryOperator 
    {
        public string SyntaxString { get; }
        public BoundBinaryOperatorKind BoundUnaryOperatorKind { get; }
        public TypeSymbol LeftType { get; }
        public TypeSymbol RightType { get; }
        public TypeSymbol ResultType { get; }

        public BoundBinaryOperator(string syntaxString, BoundBinaryOperatorKind boundUnaryOperatorKind, TypeSymbol type)
            : this(syntaxString, boundUnaryOperatorKind, type, type, type )
        {
        }
        
        public BoundBinaryOperator(string syntaxString, BoundBinaryOperatorKind boundUnaryOperatorKind, TypeSymbol operandType, TypeSymbol resultType)
            : this(syntaxString, boundUnaryOperatorKind, operandType, operandType, resultType)
        {
        }
        
        public BoundBinaryOperator(
            string syntaxString, 
            BoundBinaryOperatorKind boundUnaryOperatorKind, 
            TypeSymbol leftType,
            TypeSymbol rightType,
            TypeSymbol resultType)
        {
            SyntaxString = syntaxString;
            BoundUnaryOperatorKind = boundUnaryOperatorKind;
            LeftType = leftType;
            RightType = rightType;
            ResultType = resultType;
        }

        private static BoundBinaryOperator[] operators =
        {
            new BoundBinaryOperator(SyntaxFacts.PlusTokenString, BoundBinaryOperatorKind.Addition, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxFacts.MinusTokenString, BoundBinaryOperatorKind.Subtraction, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxFacts.StarTokenString, BoundBinaryOperatorKind.Multiplication, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxFacts.SlashTokenString, BoundBinaryOperatorKind.Division, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxFacts.PercentTokenString, BoundBinaryOperatorKind.Modulus, TypeSymbol.Int),

            new BoundBinaryOperator(SyntaxFacts.LessTokenString, BoundBinaryOperatorKind.Less, TypeSymbol.Int, TypeSymbol.Boolean),
            new BoundBinaryOperator(SyntaxFacts.LessOrEqualTokenString, BoundBinaryOperatorKind.LessOrEquals, TypeSymbol.Int, TypeSymbol.Boolean),
            new BoundBinaryOperator(SyntaxFacts.GreaterTokenString, BoundBinaryOperatorKind.Greater, TypeSymbol.Int, TypeSymbol.Boolean),
            new BoundBinaryOperator(SyntaxFacts.GreaterOrEqualTokenString, BoundBinaryOperatorKind.GreaterOrEquals, TypeSymbol.Int, TypeSymbol.Boolean),
            new BoundBinaryOperator(SyntaxFacts.EqualsEqualsTokenString, BoundBinaryOperatorKind.Equals, TypeSymbol.Int, TypeSymbol.Boolean),
            new BoundBinaryOperator(SyntaxFacts.BangEqualsTokenString, BoundBinaryOperatorKind.NotEquals, TypeSymbol.Int, TypeSymbol.Boolean),

            new BoundBinaryOperator(SyntaxFacts.EqualsEqualsTokenString, BoundBinaryOperatorKind.Equals, TypeSymbol.Boolean),
            new BoundBinaryOperator(SyntaxFacts.BangEqualsTokenString, BoundBinaryOperatorKind.NotEquals, TypeSymbol.Boolean),

        };

        public static BoundBinaryOperator? Bind(string operatorString, TypeSymbol leftOperandType, TypeSymbol rightOperandType)
        {
            var binaryOperator =
                operators.FirstOrDefault(x =>
                    x.SyntaxString == operatorString && x.LeftType == leftOperandType && x.RightType == rightOperandType);

            return binaryOperator;
        }
    
    
    }
}