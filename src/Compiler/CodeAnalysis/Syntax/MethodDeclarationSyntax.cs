using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed class MethodDeclarationSyntax : SyntaxNode
    {
        public SyntaxToken ReturnType { get; }
        public SyntaxToken MemberName { get; }
        public SyntaxToken OpenParenthesis { get; }
        public SyntaxToken CloseParenthesis { get; }
        public BlockStatementSyntax Body { get; }

        public MethodDeclarationSyntax(
            SyntaxToken returnType,
            SyntaxToken memberName,
            SyntaxToken openParenthesis,
            SyntaxToken closeParenthesis,
            BlockStatementSyntax body)
        {
            ReturnType = returnType;
            MemberName = memberName;
            OpenParenthesis = openParenthesis;
            CloseParenthesis = closeParenthesis;
            Body = body;
        }

        public override SyntaxKind Kind => SyntaxKind.MethodDeclaration;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return ReturnType;
            yield return MemberName;
            yield return OpenParenthesis;
            yield return CloseParenthesis;
            yield return Body;
        }
    }
}