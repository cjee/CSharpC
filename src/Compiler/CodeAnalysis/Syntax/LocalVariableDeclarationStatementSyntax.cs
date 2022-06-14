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
        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Type;
            yield return Identifier;

            if (EqualsToken is not null && Initializer is not null)
            {
                yield return EqualsToken;
                yield return Initializer;
            }

            yield return Semicolon;
        }
    }
}