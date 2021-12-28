using Compiler.CodeAnalysis.Binding;

namespace Compiler.CodeAnalysis.Symbols
{
    public class TypeSymbol : Symbol
    {
        public BoundExpression? DefaultInitializer { get; }
        
        public static readonly TypeSymbol Error = new TypeSymbol("?", null );
        public static readonly TypeSymbol Int = new TypeSymbol("Int", new BoundIntegralLiteralExpression(0));
        public static readonly TypeSymbol Boolean = new TypeSymbol("Boolean", new BoundBooleanLiteralExpression(false));
        public static readonly TypeSymbol Void = new TypeSymbol("Void", null);
        
        internal TypeSymbol(string name, BoundExpression? defaultInitializer) : base(name)
        {
            DefaultInitializer = defaultInitializer;
        }

        public override SymbolKind Kind => SymbolKind.Type;
    }
}