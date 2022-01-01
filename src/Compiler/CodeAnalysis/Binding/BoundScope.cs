using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Compiler.CodeAnalysis.Symbols;

namespace Compiler.CodeAnalysis.Binding;

internal class BoundScope
{
    public BoundScope? Parent { get; }
    private Dictionary<string, Symbol> symbols = new();

    public BoundScope(BoundScope? parent)
    {
        Parent = parent;
    }

    public ImmutableList<MethodSymbol> GetDeclaredMethods() => GetDeclaredSymbols<MethodSymbol>();

    private ImmutableList<T> GetDeclaredSymbols<T>() where T: Symbol
    {
        return symbols.Values.OfType<T>().ToImmutableList();
    }

    public bool TryDeclareMethod(MethodSymbol method) => TryDeclareSymbol(method);

    private bool TryDeclareSymbol<T>(T symbol) where T : Symbol
    {
        if (symbols.ContainsKey(symbol.Name))
            return false;
        
        symbols.Add(symbol.Name, symbol);
        return true;
    }

    public bool TryDeclareLocalVariable(VariableSymbol variable)
    {
        var scope = this;
        while (scope != null)
        {
            if (scope.symbols.ContainsKey(variable.Name))
                return false;
            scope = scope.Parent;
        }
        symbols.Add(variable.Name, variable);
        return true;
    }

    public bool TryLookupVariable(string name, out VariableSymbol variableSymbol)
    {
        var scope = this;
        while (scope != null)
        {
            if (scope.symbols.ContainsKey(name))
            {
                var symbol = scope.symbols[name];
                if (symbol is VariableSymbol variable)
                {
                    variableSymbol = variable;
                    return true;
                }
            }
            scope = scope.Parent;
        }

        variableSymbol = new VariableSymbol(name, TypeSymbol.Error);
        return false;
    }
}
