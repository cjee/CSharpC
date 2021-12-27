using System.Collections.Immutable;
using Compiler.CodeAnalysis.Symbols;

namespace Compiler.CodeAnalysis.Binding;

public class BoundGlobalScope 
{
    public DiagnosticBag Diagnostics { get; }
    public ImmutableList<MethodSymbol> Methods { get; }

    public BoundGlobalScope(DiagnosticBag diagnostics, ImmutableList<MethodSymbol> methods)
    {
        Diagnostics = diagnostics;
        Methods = methods;
    }
}