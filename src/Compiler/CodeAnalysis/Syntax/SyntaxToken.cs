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

public sealed record BadToken(int Position, string Text) : SyntaxToken(Position, Text, null) { public BadToken() : this(default(int), string.Empty) { } }
public sealed record EndOfFileToken(int Position) : SyntaxToken(Position, SyntaxFacts.EndOfFileTokenString, null) { public EndOfFileToken() : this(default(int)) { } }
public sealed record WhitespaceToken(int Position, string Text) : SyntaxToken(Position, Text, null) { public WhitespaceToken() : this(default(int), string.Empty) { } }
public sealed record CommentToken(int Position, string Text) : SyntaxToken(Position, Text, null) { public CommentToken() : this(default(int), string.Empty) { } }
public sealed record CharacterLiteralToken(int Position, string Text, object? Value) : SyntaxToken(Position, Text, Value) { public CharacterLiteralToken() : this(default(int), string.Empty, null) { } }
public sealed record IntegerLiteralToken(int Position, string Text, object? Value) : SyntaxToken(Position, Text, Value) { public IntegerLiteralToken() : this(default(int), string.Empty, null) { } }
public sealed record PlusToken(int Position) : SyntaxToken(Position, SyntaxFacts.PlusTokenString, null) { public PlusToken() : this(default(int)) { } }
public sealed record MinusToken(int Position) : SyntaxToken(Position, SyntaxFacts.MinusTokenString, null) { public MinusToken() : this(default(int)) { } }
public sealed record StarToken(int Position) : SyntaxToken(Position, SyntaxFacts.StarTokenString, null) { public StarToken() : this(default(int)) { } }
public sealed record SlashToken(int Position) : SyntaxToken(Position, SyntaxFacts.SlashTokenString, null) { public SlashToken() : this(default(int)) { } }
public sealed record PercentToken(int Position) : SyntaxToken(Position, SyntaxFacts.PercentTokenString, null) { public PercentToken() : this(default(int)) { } }
public sealed record BangToken(int Position) : SyntaxToken(Position, SyntaxFacts.BangTokenString, null) { public BangToken() : this(default(int)) { } }
public sealed record LessToken(int Position) : SyntaxToken(Position, SyntaxFacts.LessTokenString, null) { public LessToken() : this(default(int)) { } }
public sealed record LessOrEqualToken(int Position) : SyntaxToken(Position, SyntaxFacts.LessOrEqualTokenString, null) { public LessOrEqualToken() : this(default(int)) { } }
public sealed record GreaterToken(int Position) : SyntaxToken(Position, SyntaxFacts.GreaterTokenString, null) { public GreaterToken() : this(default(int)) { } }
public sealed record GreaterOrEqualToken(int Position) : SyntaxToken(Position, SyntaxFacts.GreaterOrEqualTokenString, null) { public GreaterOrEqualToken() : this(default(int)) { } }
public sealed record BangEqualsToken(int Position) : SyntaxToken(Position, SyntaxFacts.BangEqualsTokenString, null) { public BangEqualsToken() : this(default(int)) { } }
public sealed record EqualsEqualsToken(int Position) : SyntaxToken(Position, SyntaxFacts.EqualsEqualsTokenString, null) { public EqualsEqualsToken() : this(default(int)) { } }
public sealed record EqualsToken(int Position) : SyntaxToken(Position, SyntaxFacts.EqualsTokenString, null) { public EqualsToken() : this(default(int)) { } }
public sealed record OpenParenthesisToken(int Position) : SyntaxToken(Position, SyntaxFacts.OpenParenthesisTokenString, null) { public OpenParenthesisToken() : this(default(int)) { } }
public sealed record CloseParenthesisToken(int Position) : SyntaxToken(Position, SyntaxFacts.CloseParenthesisTokenString, null) { public CloseParenthesisToken() : this(default(int)) { } }
public sealed record OpenBraceToken(int Position) : SyntaxToken(Position, SyntaxFacts.OpenBraceTokenString, null) { public OpenBraceToken() : this(default(int)) { } }
public sealed record CloseBraceToken(int Position) : SyntaxToken(Position, SyntaxFacts.CloseBraceTokenString, null) { public CloseBraceToken() : this(default(int)) { } }
public sealed record SemicolonToken(int Position) : SyntaxToken(Position, SyntaxFacts.SemicolonTokenString, null) { public SemicolonToken() : this(default(int)) { } }
public sealed record DotToken(int Position) : SyntaxToken(Position, SyntaxFacts.DotTokenString, null) { public DotToken() : this(default(int)) { } }
public sealed record CommaToken(int Position) : SyntaxToken(Position, SyntaxFacts.CommaTokenString, null) { public CommaToken() : this(default(int)) { } }

// Keyworkds
public sealed record FalseKeyword(int Position) : SyntaxToken(Position, SyntaxFacts.FalseKeywordString, null) { public FalseKeyword() : this(default(int)) { } }
public sealed record TrueKeyword(int Position) : SyntaxToken(Position, SyntaxFacts.TrueKeywordString, null) { public TrueKeyword() : this(default(int)) { } }
public sealed record VoidKeyword(int Position) : SyntaxToken(Position, SyntaxFacts.VoidKeywordString, null) { public VoidKeyword() : this(default(int)) { } }
public sealed record BoolKeyword(int Position) : SyntaxToken(Position, SyntaxFacts.BoolKeywordString, null) { public BoolKeyword() : this(default(int)) { } }
public sealed record IntKeyword(int Position) : SyntaxToken(Position, SyntaxFacts.IntKeywordString, null) { public IntKeyword() : this(default(int)) { } }
public sealed record CharKeyword(int Position) : SyntaxToken(Position, SyntaxFacts.CharKeywordString, null) { public CharKeyword() : this(default(int)) { } }
public sealed record ReturnKeyword(int Position) : SyntaxToken(Position, SyntaxFacts.ReturnKeywordString, null) { public ReturnKeyword() : this(default(int)) { } }
public sealed record IfKeyword(int Position) : SyntaxToken(Position, SyntaxFacts.IfKeywordString, null) { public IfKeyword() : this(default(int)) { } }
public sealed record ElseKeyword(int Position) : SyntaxToken(Position, SyntaxFacts.ElseKeywordString, null) { public ElseKeyword() : this(default(int)) { } }

public sealed record Identifier(int Position, string Text, object? Value) : SyntaxToken(Position, Text, Value) { public Identifier() : this(default(int), string.Empty, null) { } }
public sealed record Type(int Position, string Text, object? Value) : SyntaxToken(Position, Text, Value) { public Type() : this(default(int), string.Empty, null) { } }