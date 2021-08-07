using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    public class TypeSyntax : SyntaxNode
    {
        public SyntaxToken Identifier { get; }

        public TypeSyntax(SyntaxToken identifier)
        {
            Identifier = identifier;
        }

        public override SyntaxKind Kind => SyntaxKind.Type;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Identifier;
        }
    }
}