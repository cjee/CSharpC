using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed record BinaryExpressionSyntax(ExpressionSyntax Left, SyntaxToken OperatorToken, ExpressionSyntax Right) : ExpressionSyntax
    {
    }
}