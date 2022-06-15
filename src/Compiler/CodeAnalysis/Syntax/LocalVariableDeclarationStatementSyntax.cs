using System;
using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed record  LocalVariableDeclarationStatementSyntax(
            TypeSyntax Type,
            SyntaxToken Identifier,
            SyntaxToken? EqualsToken,
            ExpressionSyntax? Initializer,
            SyntaxToken Semicolon) : StatementSyntax
    {
    }
}