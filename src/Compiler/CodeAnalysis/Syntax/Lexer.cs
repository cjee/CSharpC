using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    internal sealed class Lexer
    {
        private readonly List<string> diagnostics = new();
        private readonly string source;
        private SyntaxKind kind;

        private int position;
        private int start;
        private object? value;

        public Lexer(string source)
        {
            this.source = source;
        }

        private char Current => Peak(0);
        public IEnumerable<string> Diagnostics => diagnostics;

        public SyntaxToken Lex()
        {
            start = position;
            kind = SyntaxKind.BadToken;
            value = null;

            switch (Current)
            {
                case '\0':
                    kind = SyntaxKind.EndOfFileToken;
                    break;
                case '+':
                    kind = SyntaxKind.PlusToken;
                    position++;
                    break;
                case '-':
                    kind = SyntaxKind.MinusToken;
                    position++;
                    break;
                case '*':
                    kind = SyntaxKind.StarToken;
                    position++;
                    break;
                case '/':
                    kind = SyntaxKind.SlashToken;
                    position++;
                    break;
                case '(':
                    kind = SyntaxKind.OpenParenthesis;
                    position++;
                    break;
                case ')':
                    kind = SyntaxKind.CloseParenthesis;
                    position++;
                    break;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    ReadNumber();
                    break;
                case ' ':
                case '\t':
                case '\r':
                case '\n':
                    ReadWhitespace();
                    break;
                default:
                    diagnostics.Add($"Bad character at {position}: '{Current}'");
                    position++;
                    break;
            }

            var text = SyntaxFacts.GetText(kind);
            var length = position - start;
            if (text == string.Empty)
                text = source.Substring(start, length);

            return new SyntaxToken(kind, start, text, value);
        }

        private void ReadWhitespace()
        {
            while (char.IsWhiteSpace(Current))
                position++;

            kind = SyntaxKind.WhitespaceToken;
        }

        private void ReadNumber()
        {
            while (char.IsDigit(Current))
                position++;

            var length = position - start;
            var text = source.Substring(start, length);
            if (!int.TryParse(text, out var intValue))
            {
                value = null;
                diagnostics.Add("Integral constant is not a valid number.");
            }

            value = intValue;
            kind = SyntaxKind.IntegerLiteralToken;
        }

        private char Peak(int offset)
        {
            var index = position + offset;

            return index >= source.Length ? '\0' : source[index];
        }
    }
}