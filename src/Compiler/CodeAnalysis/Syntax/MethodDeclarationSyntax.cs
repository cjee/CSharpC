using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed record MethodDeclarationSyntax(
            TypeSyntax ReturnType,
            SyntaxToken MemberName,
            SyntaxToken OpenParenthesis,
            SeperatedSyntaxList<ParameterSyntax> Parameters,
            SyntaxToken CloseParenthesis,
            BlockStatementSyntax Body
        ) : SyntaxNode
    {
    }
}