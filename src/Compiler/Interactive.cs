using Compiler.CodeAnalysis;
using Compiler.CodeAnalysis.Binding;
using Compiler.CodeAnalysis.Emit;
using Compiler.CodeAnalysis.Syntax;
using System;
using System.Linq;
using System.Text;

namespace Compiler;

internal static class Interactive
{
    private static bool IsCompiled = false;
    private static StringBuilder TextBuilder = new();

    private static SyntaxTree? SyntaxTree;
    private static BoundGlobalScope? GlobalScope;
    private static BoundProgram? BoundProgram;

    private static DiagnosticBag Diagnostics = new();

    internal static void LaunchInteractive()
    {
        StartProgramInputMode();
        while (true)
        {
            Console.Write("#");
            var input = Console.ReadLine();

            switch (input)
            {
                case "print":
                    Console.WriteLine(TextBuilder.ToString());
                    break;

                case "compile":
                    CompileProgram();
                    break;

                case "syntax":
                    if (IsCompiled) PrintSyntaxTreeNode(SyntaxTree!.Root);
                    break;

                case "bound":
                    if (IsCompiled) PrintBoundProgram(BoundProgram!);
                    break;

                case "emit":
                    EmmitCodeToScreen();
                    break;

                case "cont":
                    StartProgramInputMode();
                    break;

                case "clear":
                    IsCompiled = false;
                    TextBuilder.Clear();
                    Diagnostics = new DiagnosticBag();
                    break;

                case "help":
                    PrintHelp();
                    break;

                default:
                    Console.WriteLine("Unknown command!");
                    PrintHelp();
                    break;
            }
        }
    }

    private static void EmmitCodeToScreen()
    {
        if (IsCompiled && !Diagnostics.Any())
        {
            var emiter = new Emitter(Console.Out);
            emiter.EmitGlobalScope(GlobalScope!);
            emiter.EmitBoundProgram(BoundProgram!);
        }
    }

    private static void PrintHelp()
    {
        Console.WriteLine("\t Supported commands:");
        Console.WriteLine("compile\t - print entered program;");
        Console.WriteLine("compile\t - compile program;");
        Console.WriteLine("syntax\t - print syntax tree;");
        Console.WriteLine("bound\t - print bound tree;");
        Console.WriteLine("emit\t - print c code;");
        Console.WriteLine("cont\t - continue to edit program;");
        Console.WriteLine("clear\t - clear current program;");
        Console.WriteLine("help\t - display this help';");
    }

    private static void CompileProgram()
    {
        SyntaxTree = SyntaxTree.Parse(TextBuilder.ToString());
        Diagnostics.AddRange(SyntaxTree.Diagnostics);

        GlobalScope = Binder.BindGlobalScope(SyntaxTree.Root);
        Diagnostics.AddRange(GlobalScope.Diagnostics);

        BoundProgram = Binder.BindProgram(GlobalScope);
        Diagnostics.AddRange(BoundProgram.Diagnostics);

        if (Diagnostics.Any())
        {
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (var value in Diagnostics)
            {
                Console.WriteLine($"pos: {value.Location.Start}: {value}");
            }
            Console.ResetColor();
        }
        IsCompiled = true;
    }

    private static void StartProgramInputMode()
    {
        while (true)
        {
            Console.Write(">");
            var input = Console.ReadLine();

            if (input != "#")
            {
                TextBuilder.AppendLine(input);
            }
            else
                break;
        }
    }

    private static void PrintBoundProgram(BoundProgram boundProgram)
    {
        foreach (var (symbol, body) in boundProgram.Methods)
        {
            Console.Write($"{symbol.Type.Name} {symbol.Name} (");
            Console.Write(string.Join(", ", symbol.Parameters.Select(x => $"{x.Type.Name} {x.Name}")));
            Console.WriteLine(")");
            body.WriteTo(Console.Out);
        }
    }

    private static void PrintSyntaxTreeNode(SyntaxNode node, string indent = "", bool isRoot = true,
        bool isLast = true)
    {
        Console.Write(indent);
        var marker = isLast ? "└──" : "├──";
        if (!isRoot)
            Console.Write(marker);

        Console.Write(node.NodeName);
        if (node is SyntaxToken t) Console.Write($" {t.Value}");
        Console.WriteLine();
        if (!isRoot)
            indent += isLast ? "    " : "│    ";

        var children = node.GetChildren().ToArray();
        if (children.Any())
        {
            var lastChild = children.Last();
            foreach (var child in children) PrintSyntaxTreeNode(child, indent, false, child == lastChild);
        }
    }
}