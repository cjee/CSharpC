using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Compiler;
using Compiler.CodeAnalysis;
using Compiler.CodeAnalysis.Binding;
using Compiler.CodeAnalysis.Syntax;

internal class Engine
{
    internal static void Run(Arguments arguments)
    {
        if(arguments.Files.Count != 1)
            throw new Exception("One file should be provided for compiler");
        var source = LoadSourceFile(arguments.Files[0]);

        Console.Write(source);

        var (program, diagnostics) =  Compile(source);

        if (diagnostics.Any())
        {
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (var value in diagnostics)
            {
                Console.WriteLine($"pos: {value.Location.Start}: {value}");
            }
            Console.ResetColor();
        }
        else
            Evaluate(program);
    }

    internal static (BoundProgram, DiagnosticBag) Compile(string source)
    {
        DiagnosticBag diagnostics = new();

        var tree =  SyntaxTree.Parse(source);
        diagnostics.AddRange(tree.Diagnostics);

        var globalScope = Binder.BindGlobalScope(tree.Root);
        diagnostics.AddRange(globalScope.Diagnostics);

        var boundProgram = Binder.BindProgram(globalScope);
        diagnostics.AddRange(boundProgram.Diagnostics);

        return (boundProgram, diagnostics);
    }

    internal static void Evaluate(BoundProgram boundProgram)
    {
        Evaluator evaluator = new();
        Stopwatch timer = new();
        timer.Start();
        var result = evaluator.Evaluate(boundProgram!) ?? "null";
        timer.Stop();

        Console.WriteLine($"Program exited with {result.ToString()} in {timer.ElapsedMilliseconds}ms");
    }

    private static string LoadSourceFile(string fileName)
    {
        var path = Path.Combine(Environment.CurrentDirectory, fileName);
        if(File.Exists(path))
        {
            return File.ReadAllText(path);
        }
        else
            //FIXME: handle this better;
            throw new Exception($"Proficed file does not exist: {path}");
    }
}

   