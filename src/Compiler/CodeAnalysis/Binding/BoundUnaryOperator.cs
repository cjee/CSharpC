using System.Linq;
using System.Xml.Serialization;
using Compiler.CodeAnalysis.Symbols;
using Compiler.CodeAnalysis.Syntax;

namespace Compiler.CodeAnalysis.Binding
{
    public class BoundUnaryOperator
    {
        public string SyntaxString { get; }
        public BoundUnaryOperatorKind BoundUnaryOperatorKind { get; }
        public TypeSymbol OperandType { get; }
        public TypeSymbol ResultType { get; }

        public BoundUnaryOperator(string syntaxString, BoundUnaryOperatorKind boundUnaryOperatorKind, TypeSymbol type)
            : this(syntaxString, boundUnaryOperatorKind, type, type)
        {
        }

        public BoundUnaryOperator(
            string syntaxString,
            BoundUnaryOperatorKind boundUnaryOperatorKind,
            TypeSymbol operandType,
            TypeSymbol resultType)
        {
            SyntaxString = syntaxString;
            BoundUnaryOperatorKind = boundUnaryOperatorKind;
            OperandType = operandType;
            ResultType = resultType;
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