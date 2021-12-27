namespace Compiler.CodeAnalysis.Symbols;

public class ParameterSymbol : VariableSymbol
{
    public ParameterSymbol(string name, TypeSymbol type) : base(name, type)
    {
    }

    public override SymbolKind Kind => SymbolKind.Parameter;
}