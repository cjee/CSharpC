using System;
using System.IO;

namespace Compiler
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine($"CSharpC Compiler v{typeof(Program).Assembly.GetName().Version}");
            while (true)
            {
                Console.Write(">");

                var input = Console.ReadLine();

                var lexer = new Lexer(input);
                SyntaxToken token;
                do
                {
                    token = lexer.Lex();
                    Console.WriteLine(token);
                } while (token.Kind != SyntaxKind.EndOfFileToken);
            }
        }
    }
}