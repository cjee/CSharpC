using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    public record TypeSyntax(SyntaxToken Identifier) : SyntaxNode
    {
    }
}