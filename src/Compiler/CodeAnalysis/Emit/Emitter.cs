using System;
using System.IO;
using System.Linq;
using Compiler.CodeAnalysis.Binding;
using Compiler.CodeAnalysis.Symbols;

namespace Compiler.CodeAnalysis.Emit;

public class Emitter
{
    private readonly TextWriter writer;

    public Emitter(TextWriter writer)
    {
        this.writer = writer;
    }

    public void EmitGlobalScope(BoundGlobalScope program)
    {
        foreach (var method in program.Methods)
        {
            var type = ResolveCType(method.Type);
            var methodName = method.Name;
            var parameters = string.Join(", ", method.Parameters.Select(x => $"{ResolveCType(x.Type)} {x.Name}"));
            writer.WriteLine($"{type} {methodName}({parameters});");
        }
    }

    private string ResolveCType(TypeSymbol type)
    {

        if (type == TypeSymbols.Error)
            throw new Exception("Can't emmit emit builtin Error Type");

        if (type == TypeSymbols.Boolean || type == TypeSymbols.Int)
            return "int";

        if (type == TypeSymbols.Void)
            return "void";

        throw new Exception($"Trying to emmit unknown type {type.Name}");

    }
}