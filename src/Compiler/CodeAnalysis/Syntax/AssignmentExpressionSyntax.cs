using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax;

public sealed record AssignmentExpressionSyntax(SyntaxToken Identifier, SyntaxToken OperatorToken, ExpressionSyntax Right) : ExpressionSyntax
{ 
    public override SyntaxKind Kind => SyntaxKind.AssignmentExpression;
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Identifier;
        yield return OperatorToken;
        yield return Right;
    }
}