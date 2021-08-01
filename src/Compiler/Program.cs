using System;
using System.CodeDom.Compiler;
using System.Linq;
using Compiler.CodeAnalysis;
using Compiler.CodeAnalysis.Binding;
using Compiler.CodeAnalysis.Syntax;
using Compiler.CodeAnalysis.Text;

namespace Compiler
{
    internal class Program
    {
        private static void Main()
        {
            Console.WriteLine($"CSharpC Compiler v{typeof(Program).Assembly.GetName().Version}");
            while (true)
            {
                Console.Write(">");

                var input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                    break;

                var diagnostics = new DiagnosticBag();
                var result = SyntaxTree.Parse(input);
                diagnostics.AddRange(result.Diagnostics);
                
                var binder = new Binder();
                var boundTree = binder.BindExpression(result.Root);
                diagnostics.AddRange(binder.Diagnostics);
                
                PrintSyntaxTreeNode(result.Root);
                boundTree.WriteTo(Console.Out);
                Console.WriteLine();
                
                if (diagnostics.Any())
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    foreach (var value in result.Diagnostics) Console.WriteLine($"pos: {value.Location.Start}: {value}");
                    Console.ResetColor();
                }
            }
        }

        private static void PrintSyntaxTreeNode(SyntaxNode node, string indent = "", bool isRoot = true,
            bool isLast = true)
        {
            Console.Write(indent);
            var marker = isLast ? "└──" : "├──";
            if (!isRoot)
                Console.Write(marker);


            Console.Write(node.Kind);
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
}