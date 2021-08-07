using System.Collections.Generic;
using System.Collections.Immutable;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed class CompilationUnit : SyntaxNode
    {
        public ImmutableList<MethodDeclarationSyntax> Methods { get; }
        public SyntaxToken EndOfFileToken { get; }

        public CompilationUnit(ImmutableList<MethodDeclarationSyntax> methods, SyntaxToken endOfFileToken)
        {
            Methods = methods;
            EndOfFileToken = endOfFileToken;
        }

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