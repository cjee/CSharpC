using System;

namespace Compiler
{
    public class Lexer
    {
        private readonly string source;

        private int position;
        private int start;
        private SyntaxKind kind;
        private object value;
        
        public Lexer(string source)
        {
            this.source = source;
        }

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
                case '0': case '1': case '2': case '3': case '4': 
                case '5': case '6': case '7': case '8': case '9':
                    ReadNumber();
                    break;
                case ' ': case '\t' : case '\r': case '\n' :
                    ReadWhitespace();
                    break;
                default:
                    position++;
                    break;
            }
            
            
            var text = SyntaxFacts.GetText(kind);
            var length = position - start;
            text ??= source.Substring(start, length);
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
                throw new Exception("Can't convert int");
            value = intValue;
            kind = SyntaxKind.IntegerLiteralToken;
        }

        private char Current => Peak(0);
        
        private char Peak(int offset)
        {
            var index = position + offset;

            return index >= source.Length ? '\0' : source[index];
        }
    }
}