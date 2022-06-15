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

    }
}