using Compiler.CodeAnalysis.Binding;

namespace Compiler.CodeAnalysis.Symbols;

public static class TypeSymbols
{
    public static readonly TypeSymbol Error = new TypeSymbol("?", null);
    public static readonly TypeSymbol Int = new TypeSymbol("Int", new BoundIntegralLiteralExpression(0));
    public static readonly TypeSymbol Char = new TypeSymbol("Char", new BoundCharacterLiteralExpression('\0'));
    public static readonly TypeSymbol Boolean = new TypeSymbol("Boolean", new BoundBooleanLiteralExpression(false));
    public static readonly TypeSymbol Void = new TypeSymbol("Void", null);
}