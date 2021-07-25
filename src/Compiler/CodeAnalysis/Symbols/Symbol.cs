namespace Compiler.CodeAnalysis.Symbols
{
    public abstract class Symbol
    {
        public string Name { get; }

        private protected Symbol(string name)
        {
            Name = name;
        } 
        public abstract SymbolKind Kind { get;}
    }
}