using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed record ParameterSyntax(TypeSyntax Type, SyntaxToken Identifier) : SyntaxNode
    {
        public override SyntaxKind Kind => SyntaxKind.Parameter;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Type;
            yield return Identifier;
        }
    }
}