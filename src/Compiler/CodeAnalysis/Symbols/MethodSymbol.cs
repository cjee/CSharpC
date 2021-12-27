using System.Collections.Immutable;
using Compiler.CodeAnalysis.Syntax;

namespace Compiler.CodeAnalysis.Symbols;

public class MethodSymbol : Symbol
{
    public TypeSymbol Type { get; }
    public ImmutableList<ParameterSymbol> Parameters { get; }
    public MethodDeclarationSyntax Declaration { get; }

    public  MethodSymbol(string name, TypeSymbol type,  ImmutableList<ParameterSymbol> parameters, MethodDeclarationSyntax declaration) : base(name)
    {
        Type = type;
        Parameters = parameters;
        Declaration = declaration;
    }

    public override SymbolKind Kind => SymbolKind.Method;
}