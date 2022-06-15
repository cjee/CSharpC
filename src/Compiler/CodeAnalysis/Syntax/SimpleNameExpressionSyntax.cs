using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed record SimpleNameExpressionSyntax(SyntaxToken Identifier) : ExpressionSyntax
    {
    }
}