using System.Linq;
using Compiler.CodeAnalysis.Symbols;
using Compiler.CodeAnalysis.Syntax;

namespace Compiler.CodeAnalysis.Binding
{
    public record BoundBinaryOperator (
            string SyntaxString,
            BoundBinaryOperatorKind BoundBinaryOperatorKind,
            TypeSymbol LeftType,
            TypeSymbol RightType,
            TypeSymbol ResultType)
    {
        public BoundBinaryOperator(string syntaxString, BoundBinaryOperatorKind boundBinaryOperatorKind, TypeSymbol type)
            : this(syntaxString, boundBinaryOperatorKind, type, type, type )
        {
        }
        
        public BoundBinaryOperator(string syntaxString, BoundBinaryOperatorKind boundBinaryOperatorKind, TypeSymbol operandType, TypeSymbol resultType)
            : this(syntaxString, boundBinaryOperatorKind, operandType, operandType, resultType)
        {
        }

        private static BoundBinaryOperator[] operators =
        {
            new BoundBinaryOperator(SyntaxFacts.PlusTokenString, BoundBinaryOperatorKind.Addition, TypeSymbols.Int),
            new BoundBinaryOperator(SyntaxFacts.MinusTokenString, BoundBinaryOperatorKind.Subtraction, TypeSymbols.Int),
            new BoundBinaryOperator(SyntaxFacts.StarTokenString, BoundBinaryOperatorKind.Multiplication, TypeSymbols.Int),
            new BoundBinaryOperator(SyntaxFacts.SlashTokenString, BoundBinaryOperatorKind.Division, TypeSymbols.Int),
            new BoundBinaryOperator(SyntaxFacts.PercentTokenString, BoundBinaryOperatorKind.Modulus, TypeSymbols.Int),

            new BoundBinaryOperator(SyntaxFacts.LessTokenString, BoundBinaryOperatorKind.Less, TypeSymbols.Int, TypeSymbols.Boolean),
            new BoundBinaryOperator(SyntaxFacts.LessOrEqualTokenString, BoundBinaryOperatorKind.LessOrEquals, TypeSymbols.Int, TypeSymbols.Boolean),
            new BoundBinaryOperator(SyntaxFacts.GreaterTokenString, BoundBinaryOperatorKind.Greater, TypeSymbols.Int, TypeSymbols.Boolean),
            new BoundBinaryOperator(SyntaxFacts.GreaterOrEqualTokenString, BoundBinaryOperatorKind.GreaterOrEquals, TypeSymbols.Int, TypeSymbols.Boolean),
            new BoundBinaryOperator(SyntaxFacts.EqualsEqualsTokenString, BoundBinaryOperatorKind.Equals, TypeSymbols.Int, TypeSymbols.Boolean),
            new BoundBinaryOperator(SyntaxFacts.BangEqualsTokenString, BoundBinaryOperatorKind.NotEquals, TypeSymbols.Int, TypeSymbols.Boolean),

            new BoundBinaryOperator(SyntaxFacts.EqualsEqualsTokenString, BoundBinaryOperatorKind.Equals, TypeSymbols.Boolean),
            new BoundBinaryOperator(SyntaxFacts.BangEqualsTokenString, BoundBinaryOperatorKind.NotEquals, TypeSymbols.Boolean),

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