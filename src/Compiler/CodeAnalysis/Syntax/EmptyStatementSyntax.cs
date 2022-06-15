using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed record EmptyStatementSyntax(SyntaxToken Semicolon) : StatementSyntax
    {
    }
}