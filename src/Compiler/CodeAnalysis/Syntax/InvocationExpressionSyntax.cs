using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed class InvocationExpressionSyntax : ExpressionsSyntax
    {
        public ExpressionsSyntax PrimaryExpression { get; }
        public SyntaxToken OpenParenthesis { get; }
        public SyntaxToken CloseParenthesis { get; }

        public InvocationExpressionSyntax(ExpressionsSyntax primaryExpression, SyntaxToken openParenthesis, SyntaxToken closeParenthesis)
        {
            PrimaryExpression = primaryExpression;
            OpenParenthesis = openParenthesis;
            CloseParenthesis = closeParenthesis;
        }

        public override SyntaxKind Kind => SyntaxKind.InvocationExpression;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return PrimaryExpression;
            yield return OpenParenthesis;
            yield return CloseParenthesis;
        }
    }
}