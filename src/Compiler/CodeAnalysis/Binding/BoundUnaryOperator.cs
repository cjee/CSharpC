using Compiler.CodeAnalysis.Symbols;
using Compiler.CodeAnalysis.Syntax;
using System.Linq;

namespace Compiler.CodeAnalysis.Binding
{
    public record BoundUnaryOperator(
            string SyntaxString,
            BoundUnaryOperatorKind BoundUnaryOperatorKind,
            TypeSymbol OperandType,
            TypeSymbol ResultType)
    {
        public BoundUnaryOperator(string syntaxString, BoundUnaryOperatorKind boundUnaryOperatorKind, TypeSymbol type)
            : this(syntaxString, boundUnaryOperatorKind, type, type)
        {
        }

        private static BoundUnaryOperator[] operators =
        {
            new BoundUnaryOperator(SyntaxFacts.PlusTokenString, BoundUnaryOperatorKind.Identity, TypeSymbols.Int),
            new BoundUnaryOperator(SyntaxFacts.MinusTokenString, BoundUnaryOperatorKind.Negation, TypeSymbols.Int),

            new BoundUnaryOperator(SyntaxFacts.BangTokenString, BoundUnaryOperatorKind.LogicalNegation, TypeSymbols.Boolean),
        };

        public static BoundUnaryOperator? Bind(string operandString, TypeSymbol boundOperandType)
        {
            var unaryOperator =
                operators.FirstOrDefault(x => x.SyntaxString == operandString && x.OperandType == boundOperandType);

            return unaryOperator;
        }
    }
}