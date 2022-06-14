using System.Collections.Generic;
using System.Collections.Immutable;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed record CompilationUnit(
            ImmutableList<MethodDeclarationSyntax> Methods,
            SyntaxToken EndOfFileToken
        )
        : SyntaxNode
    {
        public override SyntaxKind Kind => SyntaxKind.CompilationUnit;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            foreach (var method in Methods)
            {
                yield return method;
            }

            yield return EndOfFileToken;
        }
    }
}