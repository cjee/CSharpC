using System.Collections.Immutable;
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
}