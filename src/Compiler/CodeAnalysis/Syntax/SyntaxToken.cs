using Compiler.CodeAnalysis.Text;

namespace Compiler.CodeAnalysis.Syntax;

public abstract record SyntaxToken(int Position, string Text, object? Value) : SyntaxNode
{
    public TextSpan TextSpan => new(Position, Text.Length);

    public override TextSpan Span => TextSpan;

    public override string ToString()
    {
        return $"Pos: {Position}: Type: {this.GetType().Name}: text: '{Text}'";
    }
}

public sealed record BadToken(int Position, string Text, object? Value) : SyntaxToken(Position, Text, Value) { public BadToken() : this(default, string.Empty, null) { } }
public sealed record EndOfFileToken(int Position, string Text, object? Value) : SyntaxToken(Position, Text, Value) { public EndOfFileToken() : this(default, string.Empty, null) { } }
public sealed record WhitespaceToken(int Position, string Text, object? Value) : SyntaxToken(Position, Text, Value) { public WhitespaceToken() : this(default, string.Empty, null) { } }
public sealed record IntegerLiteralToken(int Position, string Text, object? Value) : SyntaxToken(Position, Text, Value) { public IntegerLiteralToken() : this(default, string.Empty, null) { } }

public sealed record PlusToken(int Position, string Text, object? Value) : SyntaxToken(Position, Text, Value) { public PlusToken() : this(default, string.Empty, null) { } }
public sealed record MinusToken(int Position, string Text, object? Value) : SyntaxToken(Position, Text, Value) { public MinusToken() : this(default, string.Empty, null) { } }
public sealed record StarToken(int Position, string Text, object? Value) : SyntaxToken(Position, Text, Value) { public StarToken() : this(default, string.Empty, null) { } }
public sealed record SlashToken(int Position, string Text, object? Value) : SyntaxToken(Position, Text, Value) { public SlashToken() : this(default, string.Empty, null) { } }
public sealed record PercentToken(int Position, string Text, object? Value) : SyntaxToken(Position, Text, Value) { public PercentToken() : this(default, string.Empty, null) { } }

public sealed record BangToken(int Position, string Text, object? Value) : SyntaxToken(Position, Text, Value) { public BangToken() : this(default, string.Empty, null) { } }

public sealed record LessToken(int Position, string Text, object? Value) : SyntaxToken(Position, Text, Value) { public LessToken() : this(default, string.Empty, null) { } }
public sealed record LessOrEqualToken(int Position, string Text, object? Value) : SyntaxToken(Position, Text, Value) { public LessOrEqualToken() : this(default, string.Empty, null) { } }
public sealed record GreaterToken(int Position, string Text, object? Value) : SyntaxToken(Position, Text, Value) { public GreaterToken() : this(default, string.Empty, null) { } }
public sealed record GreaterOrEqualToken(int Position, string Text, object? Value) : SyntaxToken(Position, Text, Value) { public GreaterOrEqualToken() : this(default, string.Empty, null) { } }

public sealed record BangEqualsToken(int Position, string Text, object? Value) : SyntaxToken(Position, Text, Value) { public BangEqualsToken() : this(default, string.Empty, null) { } }
public sealed record EqualsEqualsToken(int Position, string Text, object? Value) : SyntaxToken(Position, Text, Value) { public EqualsEqualsToken() : this(default, string.Empty, null) { } }

public sealed record EqualsToken(int Position, string Text, object? Value) : SyntaxToken(Position, Text, Value) { public EqualsToken() : this(default, string.Empty, null) { } }

public sealed record OpenParenthesisToken(int Position, string Text, object? Value) : SyntaxToken(Position, Text, Value) { public OpenParenthesisToken() : this(default, string.Empty, null) { } }
public sealed record CloseParenthesisToken(int Position, string Text, object? Value) : SyntaxToken(Position, Text, Value) { public CloseParenthesisToken() : this(default, string.Empty, null) { } }
public sealed record OpenBraceToken(int Position, string Text, object? Value) : SyntaxToken(Position, Text, Value) { public OpenBraceToken() : this(default, string.Empty, null) { } }
public sealed record CloseBraceToken(int Position, string Text, object? Value) : SyntaxToken(Position, Text, Value) { public CloseBraceToken() : this(default, string.Empty, null) { } }
public sealed record SemicolonToken(int Position, string Text, object? Value) : SyntaxToken(Position, Text, Value) { public SemicolonToken() : this(default, string.Empty, null) { } }
public sealed record DotToken(int Position, string Text, object? Value) : SyntaxToken(Position, Text, Value) { public DotToken() : this(default, string.Empty, null) { } }
public sealed record CommaToken(int Position, string Text, object? Value) : SyntaxToken(Position, Text, Value) { public CommaToken() : this(default, string.Empty, null) { } }

// Keyworkds
public sealed record FalseKeyword(int Position, string Text, object? Value) : SyntaxToken(Position, Text, Value) { public FalseKeyword() : this(default, string.Empty, null) { } }
public sealed record TrueKeyword(int Position, string Text, object? Value) : SyntaxToken(Position, Text, Value) { public TrueKeyword() : this(default, string.Empty, null) { } }
public sealed record VoidKeyword(int Position, string Text, object? Value) : SyntaxToken(Position, Text, Value) { public VoidKeyword() : this(default, string.Empty, null) { } }
public sealed record BoolKeyword(int Position, string Text, object? Value) : SyntaxToken(Position, Text, Value) { public BoolKeyword() : this(default, string.Empty, null) { } }
public sealed record IntKeyword(int Position, string Text, object? Value) : SyntaxToken(Position, Text, Value) { public IntKeyword() : this(default, string.Empty, null) { } }
public sealed record ReturnKeyword(int Position, string Text, object? Value) : SyntaxToken(Position, Text, Value) { public ReturnKeyword() : this(default, string.Empty, null) { } }

public sealed record Identifier(int Position, string Text, object? Value) : SyntaxToken(Position, Text, Value) { public Identifier() : this(default, string.Empty, null) { } }
public sealed record Type(int Position, string Text, object? Value) : SyntaxToken(Position, Text, Value) { public Type() : this(default, string.Empty, null) { } }