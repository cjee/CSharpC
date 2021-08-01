using System;
using Compiler.CodeAnalysis.Symbols;

namespace Compiler.CodeAnalysis.Binding
{
    internal class BoundBooleanLiteralExpression : BoundExpression
    {
        public bool Value { get; }

        public BoundBooleanLiteralExpression(bool value)
        {
            Value = value;
        }

        public override BoundNodeKind Kind => BoundNodeKind.BooleanLiteralExpression;
        public override TypeSymbol Type => TypeSymbol.Boolean;
    }
}