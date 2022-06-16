namespace Compiler.CodeAnalysis.Syntax;

internal sealed class Lexer
{
    private readonly DiagnosticBag diagnostics = new();
    private readonly string source;

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
        value = null;

        string textValue() => source.Substring(start, position - start);

        switch (Current)
        {
            case '\0':
                return new EndOfFileToken(start, textValue(), value);

            case '+':
                position++;
                return new PlusToken(start, textValue(), value);

            case '-':
                position++;
                return new MinusToken(start, textValue(), value);

            case '*':
                position++;
                return new StarToken(start, textValue(), value);

            case '/':
                position++;
                return new SlashToken(start, textValue(), value);

            case '%':
                position++;
                return new PercentToken(start, textValue(), value);

            case '!':
                position++;
                if (Current != '=')
                {
                    return new BangToken(start, textValue(), value);
                }
                else
                {
                    position++;
                    return new BangEqualsToken(start, textValue(), value);
                }
            case '<':
                position++;
                if (Current != '=')
                {
                    return new LessToken(start, textValue(), value);
                }
                else
                {
                    position++;
                    return new LessOrEqualToken(start, textValue(), value);
                }
            case '>':
                position++;
                if (Current != '=')
                {
                    return new GreaterToken(start, textValue(), value);
                }
                else
                {
                    position++;
                    return new GreaterOrEqualToken(start, textValue(), value);
                }
            case '=':
                position++;
                if (Current != '=')
                {
                    return new EqualsToken(start, textValue(), value);
                }
                else
                {
                    position++;
                    return new EqualsEqualsToken(start, textValue(), value);
                }
            case '(':
                position++;
                return new OpenParenthesisToken(start, textValue(), value);

            case ')':
                position++;
                return new CloseParenthesisToken(start, textValue(), value);

            case '{':
                position++;
                return new OpenBraceToken(start, textValue(), value);

            case '}':
                position++;
                return new CloseBraceToken(start, textValue(), value);

            case ';':
                position++;
                return new SemicolonToken(start, textValue(), value);

            case '.':
                position++;
                return new DotToken(start, textValue(), value);

            case ',':
                position++;
                return new CommaToken(start, textValue(), value);

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
                return ReadNumber();

            case ' ':
            case '\t':
            case '\r':
            case '\n':
                return ReadWhitespace();

            default:
                if (char.IsLetter(Current))
                    return ReadKeywordOrIdentifier();
                else
                {
                    diagnostics.ReportBadCharacter(position, Current);
                    position++;
                    return new BadToken(start, textValue(), value);
                }
        }
    }

    private SyntaxToken ReadKeywordOrIdentifier()
    {
        while (char.IsLetter(Current))
            position++;

        var length = position - start;
        var text = source.Substring(start, length);

        SyntaxToken token = text switch
        {
            "true" => new TrueKeyword(start, text, true),
            "false" => new FalseKeyword(start, text, false),
            "void" => new VoidKeyword(start, text, null),
            "bool" => new BoolKeyword(start, text, null),
            "int" => new IntKeyword(start, text, null),
            "return" => new ReturnKeyword(start, text, null),
            _ => new Identifier(start, text, text),
        };

        return token;
    }

    private SyntaxToken ReadWhitespace()
    {
        while (char.IsWhiteSpace(Current))
            position++;

        return new WhitespaceToken(start, source.Substring(start, position - start), null);
    }

    private SyntaxToken ReadNumber()
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
        return new IntegerLiteralToken(start, text, value);
    }

    private char Peak(int offset)
    {
        var index = position + offset;
        return index >= source.Length ? '\0' : source[index];
    }
}