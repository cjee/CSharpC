using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed class InvocationExpressionSyntax : ExpressionSyntax
    {
        public ExpressionSyntax PrimaryExpression { get; }
        public SyntaxToken OpenParenthesis { get; }
        public SeperatedSyntaxList<ExpressionSyntax> Arguments { get; }
        public SyntaxToken CloseParenthesis { get; }

        public InvocationExpressionSyntax(
            ExpressionSyntax primaryExpression,
            SyntaxToken openParenthesis,
            SeperatedSyntaxList<ExpressionSyntax> arguments,
            SyntaxToken closeParenthesis)
        {
            PrimaryExpression = primaryExpression;
            OpenParenthesis = openParenthesis;
            Arguments = arguments;
            CloseParenthesis = closeParenthesis;
        }

        public override SyntaxKind Kind => SyntaxKind.InvocationExpression;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return PrimaryExpression;
            yield return OpenParenthesis;
            foreach (var argument in Arguments)
            {
                yield return argument;
            }
            yield return CloseParenthesis;
        }
    }
}