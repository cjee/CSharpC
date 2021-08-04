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
            var metodDeclaration = ParseMethodeDeclaration();
            var endOfFileToken = MatchToken(SyntaxKind.EndOfFileToken);
            return new SyntaxTree(Diagnostics, metodDeclaration, endOfFileToken);
        }

        private MethodDeclarationSyntax ParseMethodeDeclaration()
        {
            SyntaxToken type;
            if (Current.Kind is SyntaxKind.BoolKeyword
                or SyntaxKind.IntKeyword
                or SyntaxKind.VoidKeyword
                or SyntaxKind.Identifier)
                type = NextToken();
            else
                type = MatchToken(SyntaxKind.VoidKeyword);
            
            var memberName = MatchToken(SyntaxKind.Identifier);
            var openParenthesis = MatchToken(SyntaxKind.OpenParenthesisToken);
            var closeParenthesis = MatchToken(SyntaxKind.CloseParenthesisToken);
            var body = ParseStatementBlock();

            return new MethodDeclarationSyntax(type, memberName, openParenthesis, closeParenthesis, body);
        }

        private BlockStatementSyntax ParseStatementBlock()
        {
            var openBrace = MatchToken(SyntaxKind.OpenBraceToken);

            var statementList = ImmutableList.CreateBuilder<StatementSyntax>();
            
            while (Current.Kind != SyntaxKind.CloseBraceToken && Current.Kind != SyntaxKind.EndOfFileToken)
            {
                statementList.Add(ParseStatement());
            }

            var closeBrace = MatchToken(SyntaxKind.CloseBraceToken);
            return new BlockStatementSyntax(openBrace, statementList.ToImmutable(), closeBrace);

        }

        private StatementSyntax ParseStatement()
        {
            return Current.Kind switch
            {
                SyntaxKind.OpenBraceToken => ParseStatementBlock(),
                SyntaxKind.SemicolonToken => new EmptyStatementSyntaxSyntax(NextToken()),
                
                SyntaxKind.IntKeyword or SyntaxKind.BoolKeyword => ParseDeclarationStatement(),
                SyntaxKind.ReturnKeyword => ParseReturnStatement(),
                _ => ParseExpressionStatement(),
            };
        }

        private ReturnStatementSyntax ParseReturnStatement()
        {
            var returnKeyword = NextToken();
            if(Current.Kind == SyntaxKind.SemicolonToken)
                return new ReturnStatementSyntax(returnKeyword, null, NextToken());
            
            var expression = ParseExpression();
            var semicolon = MatchToken(SyntaxKind.SemicolonToken);
            return new ReturnStatementSyntax(returnKeyword, expression, semicolon);
        }

        private StatementSyntax ParseExpressionStatement()
        {
            var expression = ParseExpression();
            var semicolon = MatchToken(SyntaxKind.SemicolonToken);
            return new ExpressionStatementSyntaxSyntax(expression, semicolon);
        }

        private StatementSyntax ParseDeclarationStatement()
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
            return new LocalVariableDeclarationStatementSyntaxSyntax(type, identifier, equalsToken, expressionsSyntax, semicolon);

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
                if (precedence == 0 || precedence < parentPrecedence )
                    break;

                // right associativity 
                if (precedence == parentPrecedence && Current.Kind != SyntaxKind.EqualsToken)
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
                case SyntaxKind.Identifier:
                    return new SimpleNameExpressionSyntax(NextToken());
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