using System.Collections.Immutable;

namespace Compiler.CodeAnalysis.Syntax;

public abstract record StatementSyntax : SyntaxNode;
public sealed record BlockStatementSyntax(SyntaxToken OpenBrace, ImmutableList<StatementSyntax> Statements, SyntaxToken CloseBrace) : StatementSyntax;
public sealed record ExpressionStatementSyntax(ExpressionSyntax Expression, SyntaxNode Semicolon) : StatementSyntax;
public sealed record LocalVariableDeclarationStatementSyntax(
        TypeSyntax Type,
        SyntaxToken Identifier,
        SyntaxToken? EqualsToken,
        ExpressionSyntax? Initializer,
        SyntaxToken Semicolon) : StatementSyntax;
public sealed record MethodDeclarationSyntax(
        TypeSyntax ReturnType,
        SyntaxToken MemberName,
        SyntaxToken OpenParenthesis,
        SeperatedSyntaxList<ParameterSyntax> Parameters,
        SyntaxToken CloseParenthesis,
        BlockStatementSyntax Body
    ) : SyntaxNode;

public sealed record ParameterSyntax(TypeSyntax Type, SyntaxToken Identifier) : SyntaxNode;
public sealed record ReturnStatementSyntax(SyntaxToken ReturnKeyword, ExpressionSyntax? Expression, SyntaxToken Semicolon) : StatementSyntax;