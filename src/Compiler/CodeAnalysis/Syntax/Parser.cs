using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Compiler.CodeAnalysis.Syntax
{
    public class Parser
    {
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
                if (token is not (WhitespaceToken or CommentToken))
                    syntaxTokens.Add(token);
            } while (token is not EndOfFileToken);

            tokens = syntaxTokens.ToArray();
            Diagnostics.AddRange(lexer.Diagnostics);
        }

        private SyntaxToken Current => Peak(0);

        public DiagnosticBag Diagnostics { get; } = new();

        private SyntaxToken Peak(int offset)
        {
            var index = position + offset;
            if (index >= tokens.Length)
                return tokens[^1];
            return tokens[index];
        }

        private SyntaxToken MatchToken<T>() where T : SyntaxToken, new()
        {
            if (Current is T)
                return NextToken();

            if (Current is BadToken)
            {
                //Bad token is already reported in diagnostics.
                //Consuming bad token and replacing with expected for enabling further processing
                position++;
                return new T() with { Position = Current.Position };
            }

            Diagnostics.ReportUnexpectedToken(Current.TextSpan, Current.GetType(), typeof(T));
            return new T() with { Position = Current.Position };
        }

        private SyntaxToken NextToken()
        {
            var current = Current;
            position++;
            return current;
        }

        public SyntaxTree Parse()
        {
            var compilationUnit = ParseCompilationUnit();
            return new SyntaxTree(Diagnostics, compilationUnit);
        }

        private CompilationUnit ParseCompilationUnit()
        {
            var builder = ImmutableList.CreateBuilder<MethodDeclarationSyntax>();
            while (Current is not EndOfFileToken)
            {
                int startposition = position;
                var method = ParseMethodeDeclaration();
                if (position == startposition)
                {
                    position++;
                }
                else
                {
                    builder.Add(method);
                }
            }

            var endOfFileToken = MatchToken<EndOfFileToken>();
            return new CompilationUnit(builder.ToImmutable(), endOfFileToken);
        }

        private MethodDeclarationSyntax ParseMethodeDeclaration()
        {
            var type = ParseType();

            var memberName = MatchToken<Identifier>();
            var openParenthesis = MatchToken<OpenParenthesisToken>();


            SeperatedSyntaxList<ParameterSyntax>? parameters = SeperatedSyntaxList<ParameterSyntax>.Empty();
            if (Current is not (CloseParenthesisToken or OpenBraceToken))
                parameters = ParseMethodeDeclarationParameters();

            var closeParenthesis = MatchToken<CloseParenthesisToken>();
            var body = ParseStatementBlock();

            return new MethodDeclarationSyntax(type, memberName, openParenthesis, parameters, closeParenthesis, body);
        }


        private SeperatedSyntaxList<ParameterSyntax> ParseMethodeDeclarationParameters()
        {
            var builder = ImmutableList.CreateBuilder<SyntaxNode>();

            var parseNextParameter = true;
            
            while (parseNextParameter && 
                   Current is not CloseBraceToken &&
                   Current is not EndOfFileToken)
            {
                var parameter = ParseParameter();
                builder.Add(parameter);
                if (Current is CommaToken)
                {
                    var comma = NextToken();
                    builder.Add(comma);
                }
                else
                {
                    parseNextParameter = false;
                }
            }

            return new SeperatedSyntaxList<ParameterSyntax>(builder.ToImmutable());
        }

        private ParameterSyntax ParseParameter()
        {
            var type = ParseType();
            var identifier = MatchToken<Identifier>();
            return new ParameterSyntax(type, identifier);
        }
        
        private TypeSyntax ParseType()
        {
            return SyntaxFacts.IsBuiltInType(Current)
                ? new TypeSyntax(NextToken())
                : new TypeSyntax(MatchToken<Identifier>());
        }


        private BlockStatementSyntax ParseStatementBlock()
        {
            var openBrace = MatchToken<OpenBraceToken>();

            var statementList = ImmutableList.CreateBuilder<StatementSyntax>();

            var startPosition = position;
            while (Current is not (CloseBraceToken or EndOfFileToken))
            {
                startPosition = position;
                statementList.Add(ParseStatement());
                if (position == startPosition)
                    position++; //Skipping tokens if we cant parse statements
            }

            var closeBrace = MatchToken<CloseBraceToken>();
            return new BlockStatementSyntax(openBrace, statementList.ToImmutable(), closeBrace);
        }

        private StatementSyntax ParseStatement()
        {
            if (Current is OpenBraceToken)
                return ParseStatementBlock();
            if (Current is SemicolonToken)
                return new EmptyStatementSyntax(NextToken());
            if (IsTypeSyntax())
                return ParseDeclarationStatement();
            if (Current is ReturnKeyword)
                return ParseReturnStatement();
            return ParseExpressionStatement();
        }

        private bool IsTypeSyntax()
        {
            return Current is IntKeyword or BoolKeyword or VoidKeyword;
        }

        private ReturnStatementSyntax ParseReturnStatement()
        {
            var returnKeyword = NextToken();
            if (Current is SemicolonToken)
                return new ReturnStatementSyntax(returnKeyword, null, NextToken());

            var expression = ParseAssignmentExpression();
            var semicolon = MatchToken<SemicolonToken>();
            return new ReturnStatementSyntax(returnKeyword, expression, semicolon);
        }

        private StatementSyntax ParseExpressionStatement()
        {
            var expression = ParseAssignmentExpression();
            var semicolon = MatchToken<SemicolonToken>();
            return new ExpressionStatementSyntax(expression, semicolon);
        }

        private ExpressionSyntax ParseAssignmentExpression()
        {
            if (Peak(0) is Identifier && Peak(1) is EqualsToken)
            {
                var identifier = NextToken();
                var operatorToken = NextToken();
                var right = ParseAssignmentExpression();
                return new AssignmentExpressionSyntax(identifier, operatorToken, right);
            }
            return ParseBinaryExpression();
        }

        private StatementSyntax ParseDeclarationStatement()
        {
            TypeSyntax type = ParseType();
            
            SyntaxToken? equalsToken = null;
            ExpressionSyntax? expressionsSyntax = null;

            SyntaxToken identifier = MatchToken<Identifier>();

            if (Current is not SemicolonToken)
            {
                equalsToken = MatchToken<EqualsToken>();

                if (Current is SemicolonToken) Diagnostics.ReportMissingExpression(Current.TextSpan);

                expressionsSyntax = ParseBinaryExpression();
            }

            SyntaxToken semicolon = MatchToken<SemicolonToken>();
            return new LocalVariableDeclarationStatementSyntax(type, identifier, equalsToken, expressionsSyntax,
                semicolon);
        }

        private ExpressionSyntax ParseBinaryExpression(int parentPrecedence = 0)
        {
            ExpressionSyntax left;
            var unaryPrecedence = Current.Text.GetUnaryOperatorPrecedence();
            if (unaryPrecedence != 0 && unaryPrecedence >= parentPrecedence)
            {
                var operatorToken = NextToken();
                var operand = ParseBinaryExpression(unaryPrecedence);
                left = new UnaryExpressionSyntax(operatorToken, operand);
            }
            else
            {
                left = ParseTerm();
            }

            while (true)
            {
                var precedence = Current.Text.GetBinaryOperatorPrecedence();
                if (precedence == 0 || precedence < parentPrecedence)
                    break;

                // right associativity 
                if (precedence == parentPrecedence && Current is not EqualsToken)
                    break;

                var operatorToken = NextToken();
                var right = ParseBinaryExpression(precedence);
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }

        private ExpressionSyntax ParseTerm()
        {
            return ParsePostfixExpression(ParsePrimaryExpression());
        }

        private ExpressionSyntax ParsePostfixExpression(ExpressionSyntax expression)
        {
            while (true)
            {
                switch (Current)
                {
                    case OpenParenthesisToken:
                        var open = NextToken();
                        var arguments = SeperatedSyntaxList<ExpressionSyntax>.Empty();
                        
                        if(Current is not CloseParenthesisToken)
                        {
                            arguments = ParseArgumentList();
                        }

                        var close = MatchToken<CloseParenthesisToken>();
                        expression = new InvocationExpressionSyntax(expression, open, arguments, close);
                        break;
                    default:
                        return expression;
                }
            }
        }

        private SeperatedSyntaxList<ExpressionSyntax> ParseArgumentList()
        {
            var builder = ImmutableList.CreateBuilder<SyntaxNode>();
            
            var parseNextParameter = true;
            
            while (parseNextParameter && 
                   Current is not CloseBraceToken &&
                   Current is not EndOfFileToken)
            {
                var parameter = ParseBinaryExpression();
                builder.Add(parameter);
                if (Current is CommaToken)
                {
                    var comma = NextToken();
                    builder.Add(comma);
                }
                else
                {
                    parseNextParameter = false;
                }
            }

            return new SeperatedSyntaxList<ExpressionSyntax>(builder.ToImmutable());
        }

        private ExpressionSyntax ParsePrimaryExpression()
        {
            switch (Current)
            {
                case OpenParenthesisToken:
                {
                    var open = NextToken();
                    var expression = ParseBinaryExpression();
                    var close = MatchToken<CloseParenthesisToken>();
                    return new ParenthesizedExpressionSyntax(open, expression, close);
                }
                case CharacterLiteralToken:
                    return new CharacterLiteralExpressionSyntax(NextToken());
                case FalseKeyword or TrueKeyword:
                    return new BooleanLiteralExpressionSyntax(NextToken());
                case IntegerLiteralToken:
                    return new NumericLiteralExpressionSyntax(NextToken());
                case Identifier:
                default:
                    return new SimpleNameExpressionSyntax(MatchToken<Identifier>());
            }
        }
    }
}