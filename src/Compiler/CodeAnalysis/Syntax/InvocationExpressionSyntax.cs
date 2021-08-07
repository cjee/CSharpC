using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    public sealed class InvocationExpressionSyntax : ExpressionsSyntax
    {
        public ExpressionsSyntax PrimaryExpression { get; }
        public SyntaxToken OpenParenthesis { get; }
        public SeperatedSyntaxList<ExpressionsSyntax> Arguments { get; }
        public SyntaxToken CloseParenthesis { get; }

        public InvocationExpressionSyntax(
            ExpressionsSyntax primaryExpression,
            SyntaxToken openParenthesis,
            SeperatedSyntaxList<ExpressionsSyntax> arguments,
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