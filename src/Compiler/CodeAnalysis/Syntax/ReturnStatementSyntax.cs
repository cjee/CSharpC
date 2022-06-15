using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed record ReturnStatementSyntax(SyntaxToken ReturnKeyword, ExpressionSyntax? Expression, SyntaxToken Semicolon) : StatementSyntax
    {
    }
}