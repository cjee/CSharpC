using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    internal sealed class Lexer
    {
        private readonly DiagnosticBag  diagnostics= new();
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
        public DiagnosticBag Diagnostics => diagnostics;

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
                case '%':
                    kind = SyntaxKind.PercentToken;
                    position++;
                    break;
                case '!':
                    position++;
                    if (Current != '=')
                    {
                        kind = SyntaxKind.BangToken;
                    }
                    else
                    {
                        kind = SyntaxKind.BangEqualsToken;
                        position++;
                    }
                    break;
                case '<':
                    position++;
                    if (Current != '=')
                    {
                        kind = SyntaxKind.LessToken;
                    }
                    else
                    {
                        position++;
                        kind = SyntaxKind.LessOrEqualToken;
                    }
                    break;
                case '>':
                    position++;
                    if (Current != '=')
                    {
                        kind = SyntaxKind.GreaterToken;
                    }
                    else
                    {
                        position++;
                        kind = SyntaxKind.GreaterOrEqualToken;
                    }
                    break;
                case '=':
                    position++;
                    if (Current != '=')
                    {
                        kind = SyntaxKind.EqualsToken;
                    }
                    else
                    {
                        kind = SyntaxKind.EqualsEqualsToken;
                        position++;
                    }
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
                    if (char.IsLetter(Current))
                        ReadKeyword();
                    else
                    {
                        diagnostics.ReportBadCharacter(position, Current);
                        position++;
                    }
                    break;
            }

            var text = SyntaxFacts.GetText(kind);
            var length = position - start;
            if (text == string.Empty)
                text = source.Substring(start, length);

            return new SyntaxToken(kind, start, text, value);
        }

        private void ReadKeyword()
        {
            while (char.IsLetter(Current))
                position++;

            var length = position - start;
            var text = source.Substring(start, length);

            kind = SyntaxFacts.GetKeywordToken(text);
            value = kind switch
            {
                SyntaxKind.TrueKeyword => true,
                SyntaxKind.FalseKeyword => false,
                _ => null,
            };

            if (kind is SyntaxKind.BadToken)
                diagnostics.ReportBadToken(start, length, text);

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
                diagnostics.ReportBadIntegral(start, length);
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