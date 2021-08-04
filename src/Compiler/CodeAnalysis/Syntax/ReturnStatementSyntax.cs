using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    internal class ReturnStatementSyntax : StatementSyntax
    {
        public SyntaxToken ReturnKeyword { get; }
        public ExpressionsSyntax? Expression { get; }
        public SyntaxToken Semicolon { get; }

        public ReturnStatementSyntax(SyntaxToken returnKeyword, ExpressionsSyntax? expression, SyntaxToken semicolon)
        {
            ReturnKeyword = returnKeyword;
            Expression = expression;
            Semicolon = semicolon;
        }

        public override SyntaxKind Kind => SyntaxKind.ReturnStatement;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return ReturnKeyword;
            if (Expression is not null)
                yield return Expression;
            yield return Semicolon;
        }
    }
}