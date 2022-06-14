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
        public override SyntaxKind Kind => SyntaxKind.MethodDeclaration;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return ReturnType;
            yield return MemberName;
            yield return OpenParenthesis;

            foreach (var parameter in Parameters)
            {
                yield return parameter;
            }

            yield return CloseParenthesis;
            yield return Body;
        }
    }
}