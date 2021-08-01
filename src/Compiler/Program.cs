using System;
using System.CodeDom.Compiler;
using System.Linq;
using Compiler.CodeAnalysis.Binding;
using Compiler.CodeAnalysis.Syntax;

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

                var result = SyntaxTree.Parse(input);
                var binder = new Binder();
                var boundTree = binder.BindExpression(result.Root);
                
                
                PrintSyntaxTreeNode(result.Root);
                boundTree.WriteTo(Console.Out);
                Console.WriteLine();
                
                if (result.Diagnostics.Any())
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    foreach (var value in result.Diagnostics) Console.WriteLine(value);
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