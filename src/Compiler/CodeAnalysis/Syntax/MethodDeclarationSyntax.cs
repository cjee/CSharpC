using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed class MethodDeclarationSyntax : SyntaxNode
    {
        public TypeSyntax ReturnType { get; }
        public SyntaxToken MemberName { get; }
        public SyntaxToken OpenParenthesis { get; }
        public SeperatedSyntaxList<ParameterSyntax> Parameters { get; }
        public SyntaxToken CloseParenthesis { get; }
        public BlockStatementSyntax Body { get; }

        public MethodDeclarationSyntax(
            TypeSyntax returnType,
            SyntaxToken memberName,
            SyntaxToken openParenthesis,
            SeperatedSyntaxList<ParameterSyntax> parameters,
            SyntaxToken closeParenthesis,
            BlockStatementSyntax body)
        {
            ReturnType = returnType;
            MemberName = memberName;
            OpenParenthesis = openParenthesis;
            Parameters = parameters;
            CloseParenthesis = closeParenthesis;
            Body = body;
        }

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