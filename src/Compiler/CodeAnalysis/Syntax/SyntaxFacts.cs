namespace Compiler.CodeAnalysis.Syntax;

public static class SyntaxFacts
{
    public const string EndOfFileTokenString = "\0";
    public const string PlusTokenString = "+";
    public const string MinusTokenString = "-";
    public const string StarTokenString = "*";
    public const string SlashTokenString = "/";
    public const string PercentTokenString = "%";
    public const string BangTokenString = "!";
    public const string LessTokenString = "<";
    public const string LessOrEqualTokenString = "<=";
    public const string GreaterTokenString = ">";
    public const string GreaterOrEqualTokenString = ">=";
    public const string BangEqualsTokenString = "!=";
    public const string EqualsEqualsTokenString = "==";
    public const string EqualsTokenString = "=";
    public const string OpenParenthesisTokenString = "(";
    public const string CloseParenthesisTokenString = ")";
    public const string OpenBraceTokenString = "{";
    public const string CloseBraceTokenString = "}";
    public const string SemicolonTokenString = ";";
    public const string DotTokenString = ".";
    public const string CommaTokenString = ",";

    public const string FalseKeywordString = "false";
    public const string TrueKeywordString = "true";
    public const string VoidKeywordString = "void";
    public const string BoolKeywordString = "bool";
    public const string CharKeywordString = "char";
    public const string IntKeywordString = "int";
    public const string ReturnKeywordString = "return";
    
    public static int GetUnaryOperatorPrecedence(this string syntaxText)
    {
        switch (syntaxText)
        {
            case PlusTokenString:
            case MinusTokenString:
            case BangTokenString:
                return 13; //Based on C# operator count
            default:
                return 0;
        }
    }

    public static int GetBinaryOperatorPrecedence(this string syntaxText)
    {
        switch (syntaxText)
        {
            case StarTokenString:
            case SlashTokenString:
            case PercentTokenString:
                return 12;

            case PlusTokenString:
            case MinusTokenString:
                return 11;

            case LessTokenString:
            case LessOrEqualTokenString:
            case GreaterTokenString:
            case GreaterOrEqualTokenString:
                return 9;

            case BangEqualsTokenString:
            case EqualsEqualsTokenString:
                return 8;

            case EqualsTokenString:
                return 1;

            default:
                return 0;
        }
    }

    public static bool IsBuiltInType(SyntaxNode currentKind)
    {
        return currentKind switch
        {
            VoidKeyword => true,
            BoolKeyword => true,
            IntKeyword => true,
            CharKeyword => true,
            _ => false,
        };
    }
}