using Compiler.CodeAnalysis.Symbols;

namespace Compiler.CodeAnalysis.Binding
{
    public abstract class BoundExpression : BoundNode
    {
        public abstract TypeSymbol Type { get; }
    }
}