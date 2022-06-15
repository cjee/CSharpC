using System.Collections.Generic;
using System.Collections.Immutable;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed record BlockStatementSyntax(SyntaxToken OpenBrace, ImmutableList<StatementSyntax> Statements, SyntaxToken CloseBrace) : StatementSyntax
    {
    }
}