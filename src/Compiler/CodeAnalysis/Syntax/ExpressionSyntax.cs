namespace Compiler.CodeAnalysis.Syntax;

public abstract record ExpressionSyntax : SyntaxNode;
public sealed record AssignmentExpressionSyntax(SyntaxToken Identifier, SyntaxToken OperatorToken, ExpressionSyntax Right) : ExpressionSyntax;
public sealed record BinaryExpressionSyntax(ExpressionSyntax Left, SyntaxToken OperatorToken, ExpressionSyntax Right) : ExpressionSyntax;
public sealed record BooleanLiteralExpressionSyntax(SyntaxToken BooleanToken) : ExpressionSyntax;
public sealed record EmptyStatementSyntax(SyntaxToken Semicolon) : StatementSyntax;
public sealed record InvocationExpressionSyntax(
        ExpressionSyntax PrimaryExpression,
        SyntaxToken OpenParenthesis,
        SeperatedSyntaxList<ExpressionSyntax> Arguments,
        SyntaxToken CloseParenthesis) : ExpressionSyntax;

public sealed record NumericLiteralExpressionSyntax(SyntaxToken NumberToken) : ExpressionSyntax;
public sealed record CharacterLiteralExpressionSyntax(SyntaxToken CharacterToken) : ExpressionSyntax;
public sealed record ParenthesizedExpressionSyntax(SyntaxToken OpenParenthesis, ExpressionSyntax Expression, SyntaxToken CloseParenthesis) : ExpressionSyntax;
public sealed record SimpleNameExpressionSyntax(SyntaxToken Identifier) : ExpressionSyntax;
public sealed record UnaryExpressionSyntax(SyntaxToken OperatorToken, ExpressionSyntax Operand) : ExpressionSyntax;