using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    public class ParameterSyntax : SyntaxNode
    {
        public TypeSyntax Type { get; }
        public SyntaxToken Identifier { get; }

        public ParameterSyntax(TypeSyntax type, SyntaxToken identifier)
        {
            Type = type;
            Identifier = identifier;
        }

        public override SyntaxKind Kind => SyntaxKind.Parameter;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Type;
            yield return Identifier;
        }
    }
}