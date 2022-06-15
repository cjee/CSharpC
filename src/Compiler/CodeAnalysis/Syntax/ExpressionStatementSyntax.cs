using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed record ExpressionStatementSyntax(ExpressionSyntax Expression, SyntaxNode Semicolon) : StatementSyntax
    {
    }
}