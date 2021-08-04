namespace Compiler.CodeAnalysis.Symbols
{
    public class TypeSymbol : Symbol
    {
        public static readonly TypeSymbol Error = new TypeSymbol("?");
        public static readonly TypeSymbol Int = new TypeSymbol("Int");
        public static readonly TypeSymbol Boolean = new TypeSymbol("Boolean");
        public static readonly TypeSymbol Void = new TypeSymbol("Void");
        
        internal TypeSymbol(string name) : base(name)
        {
        }

        public override SymbolKind Kind => SymbolKind.Type;
    }
}