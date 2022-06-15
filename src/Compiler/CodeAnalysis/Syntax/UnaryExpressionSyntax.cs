using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed record UnaryExpressionSyntax(SyntaxToken OperatorToken, ExpressionSyntax Operand) : ExpressionSyntax
    {
    }
}