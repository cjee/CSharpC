using System.Collections.Immutable;
using System.Linq;
using Compiler.CodeAnalysis.Symbols;

namespace Compiler.CodeAnalysis.Binding;

public class BoundProgram
{
    public DiagnosticBag Diagnostics { get; }
    public ImmutableDictionary<MethodSymbol, BoundBlockStatement> Methods { get; }

    public BoundProgram(DiagnosticBag diagnostics, ImmutableDictionary<MethodSymbol, BoundBlockStatement> methods)
    {
        Diagnostics = diagnostics;
        Methods = methods;
    }

    public bool HasErrors => Diagnostics.Any();
}