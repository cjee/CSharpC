using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax
{
    public class Parser
    {
        private readonly List<string> diagnostics = new();
        private readonly SyntaxToken[] tokens;
        private int position;

        public Parser(string text)
        {
            Lexer lexer = new(text);
            var syntaxTokens = new List<SyntaxToken>();
            SyntaxToken token;
            do
            {
                token = lexer.Lex();
                if (token.Kind != SyntaxKind.BadToken && token.Kind != SyntaxKind.WhitespaceToken)
                    syntaxTokens.Add(token);
            } while (token.Kind != SyntaxKind.EndOfFileToken);

            tokens = syntaxTokens.ToArray();
            diagnostics.AddRange(lexer.Diagnostics);
        }

        private SyntaxToken Current => Peak(0);

        public IEnumerable<string> Diagnostics => diagnostics;

        private SyntaxToken Peak(int offset)
        {
            var index = position + offset;
            if (index >= tokens.Length)
                return tokens[^1];
            return tokens[index];
        }

        private SyntaxToken MatchToken(SyntaxKind kind)
        {
            if (Current.Kind == kind)
                return NextToken();

            diagnostics.Add($"Unexpected token <{Current.Kind}>, expected <{kind}>");
            return new SyntaxToken(kind, Current.Position, string.Empty, null);
        }

        private SyntaxToken NextToken()
        {
            var current = Current;
            position++;
            return current;
        }

        public SyntaxTree Parse()
        {
            var expression = ParseExpression();
            var endOfFileToken = MatchToken(SyntaxKind.EndOfFileToken);
            return new SyntaxTree(Diagnostics, expression, endOfFileToken);
        }

        private ExpressionsSyntax ParseExpression(int parentPrecedence = 0)
        {
            ExpressionsSyntax left;
            var unaryPrecedence = Current.Kind.GetUnaryOperatorPrecedence();
            if (unaryPrecedence != 0 && unaryPrecedence >= parentPrecedence)
            {
                var operatorToken = NextToken();
                var operand = ParseExpression(unaryPrecedence);
                left = new UnaryExpressionSyntax(operatorToken, operand);
            }
            else
            {
                left = ParsePrimaryExpression();
            }
            
            while (true)
            {
                var precedence = Current.Kind.GetBinaryOperatorPrecedence();
                if (precedence == 0 || precedence <= parentPrecedence)
                    break;
                
                var operatorToken = NextToken();
                var right = ParseExpression(precedence);
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }
            return left;
        }

        private ExpressionsSyntax ParsePrimaryExpression()
        {
            if (Current.Kind is SyntaxKind.OpenParenthesis)
            {
                var open = NextToken();
                var expression = ParseExpression();
                var close = MatchToken(SyntaxKind.CloseParenthesis);
                return new ParenthesizedExpressionSyntax(open, expression, close);
            }

            return new NumericLiteralExpressionSyntax(MatchToken(SyntaxKind.IntegerLiteralToken));
        }
    }
}