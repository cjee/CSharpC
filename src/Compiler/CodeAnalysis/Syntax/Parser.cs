using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Compiler.CodeAnalysis.Syntax
{
    public class Parser
    {
        private readonly DiagnosticBag diagnostics = new();
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
                if (token.Kind != SyntaxKind.WhitespaceToken)
                    syntaxTokens.Add(token);
            } while (token.Kind != SyntaxKind.EndOfFileToken);

            tokens = syntaxTokens.ToArray();
            diagnostics.AddRange(lexer.Diagnostics);
        }

        private SyntaxToken Current => Peak(0);

        public DiagnosticBag Diagnostics => diagnostics;

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

            if (Current.Kind == SyntaxKind.BadToken)
            {
                //Bad token is already reported in diagnostics.
                //Consuming bad token and replacing with expected for enabling further processing
                position++;
                return new SyntaxToken(kind, Current.Position, string.Empty, null);
            }
            
            Diagnostics.ReportUnexpectedToken(Current.TextSpan, Current.Kind, kind);
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
            var statementBlock = ParseStatementBlock();
            var endOfFileToken = MatchToken(SyntaxKind.EndOfFileToken);
            return new SyntaxTree(Diagnostics, statementBlock, endOfFileToken);
        }

        private BlockStatement ParseStatementBlock()
        {
            var openBrace = MatchToken(SyntaxKind.OpenBraceToken);

            var statementList = ImmutableList.CreateBuilder<Statement>();
            
            while (Current.Kind != SyntaxKind.CloseBraceToken)
            {
                statementList.Add(ParseStatement());
            }

            var closeBrace = MatchToken(SyntaxKind.CloseBraceToken);
            return new BlockStatement(openBrace, statementList.ToImmutable(), closeBrace);

        }

        private Statement ParseStatement()
        {
            return Current.Kind switch
            {
                SyntaxKind.OpenBraceToken => ParseStatementBlock(),
                SyntaxKind.SemicolonToken => new EmptyStatement(NextToken()),
                SyntaxKind.IntKeyword or SyntaxKind.BoolKeyword => ParseDeclarationStatement(),
                _ => throw new NotImplementedException($"Not all statement types are parsable {Current.Kind}"),
            };
        }

        private Statement ParseDeclarationStatement()
        {
            SyntaxToken type = NextToken();
            SyntaxToken? equalsToken = null;
            ExpressionsSyntax? expressionsSyntax = null;

            SyntaxToken identifier = MatchToken(SyntaxKind.Identifier);

            if (Current.Kind != SyntaxKind.SemicolonToken)
            {
                equalsToken = MatchToken(SyntaxKind.EqualsToken);
                
                if (Current.Kind == SyntaxKind.SemicolonToken)
                {
                    diagnostics.ReportMissingExpression(Current.TextSpan);
                }

                expressionsSyntax = ParseExpression();
            }

            SyntaxToken semicolon = MatchToken(SyntaxKind.SemicolonToken);
            return new LocalVariableDeclarationStatement(type, identifier, equalsToken, expressionsSyntax, semicolon);

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
            switch (Current.Kind)
            {
                case SyntaxKind.OpenParenthesisToken:
                {
                    var open = NextToken();
                    var expression = ParseExpression();
                    var close = MatchToken(SyntaxKind.CloseParenthesisToken);
                    return new ParenthesizedExpressionSyntax(open, expression, close);
                }
                case SyntaxKind.FalseKeyword or SyntaxKind.TrueKeyword:
                    return new BooleanLiteralExpressionSyntax(NextToken());
                case SyntaxKind.IntegerLiteralToken:
                    return new NumericLiteralExpressionSyntax(NextToken());
                default:
                    return new EmptyExpressionSyntax();
            }
        }
    }
}