using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed record ParameterSyntax(TypeSyntax Type, SyntaxToken Identifier) : SyntaxNode
    {
    }
}