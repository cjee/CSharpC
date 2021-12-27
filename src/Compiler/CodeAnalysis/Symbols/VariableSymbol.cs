namespace Compiler.CodeAnalysis.Symbols;

public class VariableSymbol : Symbol
{
    public TypeSymbol Type { get; }

    public VariableSymbol(string name, TypeSymbol type) : base(name)
    {
        Type = type;
    }

    public override SymbolKind Kind => SymbolKind.Variable;
}