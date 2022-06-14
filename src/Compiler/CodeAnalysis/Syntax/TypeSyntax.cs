using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    public record TypeSyntax(SyntaxToken Identifier) : SyntaxNode
    {
        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Identifier;
        }
    }
}