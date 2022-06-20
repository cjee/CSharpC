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
    private static StringBuilder TextBuilder = new();
    private static BoundProgram? BoundProgram;

    internal static void LaunchInteractive()
    {
        StartProgramInputMode();
        while (true)
        {
            Console.Write("#");
            var inputLine = Console.ReadLine() ?? string.Empty;
            var inputs = inputLine.Split(" ");
            var command = inputs[0];
            switch (command)
            { 
                case "load":
                    TextBuilder.Append(Engine.LoadSourceFile(inputs[1]));
                    Compile();
                    break;

                case "print":
                    Console.WriteLine(TextBuilder.ToString());
                    break;

                case "syntax":
                        var tree =  SyntaxTree.Parse(TextBuilder.ToString());
                        ReportDiagnostics(tree.Diagnostics);
                        PrintSyntaxTreeNode(tree.Root);
                    break;

                case "bound":
                    if (BoundProgram is not null)
                        BoundProgram!.PrintBoundProgram(Console.Out);
                    break;

                case "eval":
                    if(BoundProgram is not null)
                        Engine.Evaluate(BoundProgram!);
                    break;

                case "emit":
                    EmmitCodeToScreen();
                    break;

                case "cont":
                    StartProgramInputMode();
                    break;

                case "clear":
                    TextBuilder.Clear();
                    BoundProgram = null;
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

    private static void Compile()
    {
        var (program, diagnostics) = Engine.Compile(TextBuilder.ToString());
        ReportDiagnostics(diagnostics);
        BoundProgram = program;
    }

    private static void EmmitCodeToScreen()
    {
        if (BoundProgram is not null && !BoundProgram.HasErrors)
        {
            var emiter = new Emitter(Console.Out);
            emiter.EmitGlobalScope(BoundProgram);
            emiter.EmitBoundProgram(BoundProgram);
        }
    }

    private static void PrintHelp()
    {
        Console.WriteLine("\t Supported commands:");
        Console.WriteLine("load [filename]\t - load program from file;");
        Console.WriteLine("print\t - print entered program;");
        Console.WriteLine("syntax\t - print syntax tree;");
        Console.WriteLine("bound\t - print bound tree;");
        Console.WriteLine("eval\t - Evaluate program;");
        Console.WriteLine("emit\t - print c code;");
        Console.WriteLine("cont\t - continue to edit program;");
        Console.WriteLine("clear\t - clear current program;");
        Console.WriteLine("help\t - display this help';");
    }

    private static void ReportDiagnostics(DiagnosticBag diagnostics)
    {
        if (diagnostics.Any())
        {
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (var value in diagnostics)
            {
                Console.WriteLine($"pos: {value.Location.Start}: {value}");
            }
            Console.ResetColor();
        }
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
        Compile();
    }

    private static void PrintSyntaxTreeNode(SyntaxNode node, string indent = "", bool isRoot = true,
        bool isLast = true, SyntaxNode? parrent = null)
    {
        var currentColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(indent);
        var marker = isLast ? "└──" : "├──";
        if (!isRoot)
            Console.Write(marker);

        Console.ForegroundColor = node switch
        {
            TypeSyntax => typeColor,
            Identifier when parrent is MethodDeclarationSyntax => identifierColor,
            Identifier when parrent is LocalVariableDeclarationStatementSyntax or ParameterSyntax or SimpleNameExpressionSyntax => localColor,
            NumericLiteralExpressionSyntax or IntegerLiteralToken => integerColor,
            ReturnKeyword => keywordColor,
            _ => currentColor,
        };

        Console.Write(node.NodeName);
        if (node is SyntaxToken t) Console.Write($" {t.Value}");
        Console.WriteLine();


        if (!isRoot)
            indent += isLast ? "    " : "│    ";

        var children = node.GetChildren().ToArray();
        if (children.Any())
        {
            var lastChild = children.Last();
            foreach (var child in children) PrintSyntaxTreeNode(child, indent, false, child == lastChild, node);
        }

        Console.ResetColor();
    }

    private static ConsoleColor typeColor = ConsoleColor.DarkBlue;
    private static ConsoleColor identifierColor = ConsoleColor.Yellow;
    private static ConsoleColor localColor = ConsoleColor.Blue;
    private static ConsoleColor integerColor = ConsoleColor.Green;
    private static ConsoleColor keywordColor = ConsoleColor.Magenta;

}