using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax;

internal class AssignmentExpressionSyntax : ExpressionSyntax
{
    public SyntaxToken Identifier { get; }
    public SyntaxToken OperatorToken { get; }
    public ExpressionSyntax Right { get; }

    public AssignmentExpressionSyntax(SyntaxToken identifier, SyntaxToken operatorToken, ExpressionSyntax right)
    {
        Identifier = identifier;
        OperatorToken = operatorToken;
        Right = right;
    }

    public override SyntaxKind Kind => SyntaxKind.AssignmentExpression;
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Identifier;
        yield return OperatorToken;
        yield return Right;
    }
}