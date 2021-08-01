using System.Linq;
using System.Xml.Serialization;
using Compiler.CodeAnalysis.Symbols;
using Compiler.CodeAnalysis.Syntax;

namespace Compiler.CodeAnalysis.Binding
{
    public class BoundUnaryOperator
    {
        public SyntaxKind SyntaxKind { get; }
        public BoundUnaryOperatorKind BoundUnaryOperatorKind { get; }
        public TypeSymbol OperandType { get; }
        public TypeSymbol ResultType { get; }

        public BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind boundUnaryOperatorKind, TypeSymbol type)
            : this(syntaxKind, boundUnaryOperatorKind, type, type)
        {
        }

        public BoundUnaryOperator(
            SyntaxKind syntaxKind,
            BoundUnaryOperatorKind boundUnaryOperatorKind,
            TypeSymbol operandType,
            TypeSymbol resultType)
        {
            SyntaxKind = syntaxKind;
            BoundUnaryOperatorKind = boundUnaryOperatorKind;
            OperandType = operandType;
            ResultType = resultType;
        }

        private static BoundUnaryOperator[] operators =
        {
            new BoundUnaryOperator(SyntaxKind.PlusToken, BoundUnaryOperatorKind.Identity, TypeSymbol.Int),
            new BoundUnaryOperator(SyntaxKind.MinusToken, BoundUnaryOperatorKind.Negation, TypeSymbol.Int),
        };

        public static BoundUnaryOperator? Bind(SyntaxKind operandKind, TypeSymbol boundOperandType)
        {
            var unaryOperator =
                operators.FirstOrDefault(x => x.SyntaxKind == operandKind && x.OperandType == boundOperandType);

            return unaryOperator;
        }
    }
}