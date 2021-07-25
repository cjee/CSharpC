using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed class ParenthesizedExpressionSyntax : ExpressionsSyntax
    {
        public ParenthesizedExpressionSyntax(
            SyntaxToken openParenthesis,
            ExpressionsSyntax expression,
            SyntaxToken closeParenthesis)
        {
            OpenParenthesis = openParenthesis;
            Expression = expression;
            CloseParenthesis = closeParenthesis;
        }

        public SyntaxToken OpenParenthesis { get; }
        public ExpressionsSyntax Expression { get; }
        public SyntaxToken CloseParenthesis { get; }
        public override SyntaxKind Kind => SyntaxKind.ParenthesizedExpression;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OpenParenthesis;
            yield return Expression;
            yield return CloseParenthesis;
        }
    }
}