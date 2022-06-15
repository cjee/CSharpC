using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed record InvocationExpressionSyntax(
            ExpressionSyntax PrimaryExpression,
            SyntaxToken OpenParenthesis,
            SeperatedSyntaxList<ExpressionSyntax> Arguments,
            SyntaxToken CloseParenthesis) : ExpressionSyntax
    {
    }
}